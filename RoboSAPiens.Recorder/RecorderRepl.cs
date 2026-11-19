using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace RoboSAPiens.Recorder
{
    public static class Repl
    {
        public static string validatedReadInput(string prompt, Func<string, bool> isValid, string validationError)
        {
            string readInput(string prompt)
            {
                Console.InputEncoding = Encoding.Unicode;
                Console.Write(prompt);
                return Console.ReadLine()!.Trim();
            }

            string input = readInput(prompt);
            while (!isValid(input))
            {
                Console.WriteLine(validationError);
                input = readInput(prompt);
            }
            return input;
        }

        public record Command(string name, string description, Action action);

        public abstract record BaseRepl
        {
            protected virtual string prompt()
            {
                return "> ";
            }

            Dictionary<string, Command> commandDict()
            {
                return getCommands().ToDictionary(c => c.name, c => c);
            }

            bool isValidCommand(string command)
            {
                return commandDict().ContainsKey(command) || command.StartsWith('$');
            }

            protected virtual List<string> getBanner()
            {
                return
                [
                    "Type `help` to get the list of available commands."
                ];
            }

            protected virtual List<Command> getCommands()
            {
                return
                [   
                    new Command("exit", "Exit the program", () => Environment.Exit(0)),
                    new Command("help", "Show this help message", help)
                ];
            }

            protected virtual void handleCommand(string command)
            {
                commandDict().GetValueOrDefault(command)?.action();
            }

            protected void help()
            {
                Console.WriteLine("Available commands:");
                foreach (var command in getCommands())
                {
                    Console.WriteLine($"  {command.name}  - {command.description}");
                }
            }

            public void repl()
            {
                getBanner().ForEach(Console.WriteLine);

                while (true)
                {
                    try
                    {
                        handleCommand(validatedReadInput(prompt(), isValidCommand, "Invalid command."));
                    }
                    catch (NoSapException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (Exception e) 
                    {
                        Console.WriteLine();
                        Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                    }
                }
            }
        }

        public static class State
        {
            public abstract record Idle(Recorder.State.Initialized recorder): BaseRepl
            {
                void newTestCase()
                {
                    string testcaseName = validatedReadInput(
                        "Test Case: ", 
                        testcase => !string.IsNullOrWhiteSpace(testcase), 
                        "Test case name cannot be empty."
                    );
                    string lang = validatedReadInput(
                        "Language [en, de]: ", 
                        lang => lang == "en" || lang == "de",
                        "Invalid language."
                    );
                    record(lang, testcaseName).repl();
                }

                protected override List<string> getBanner()
                {
                    return 
                    [
                        "=============== RoboSAPiens Recorder CLI ===============",
                        recorder.recordingMode.ToString()!,
                        ..base.getBanner()
                    ];
                }

                protected override List<Command> getCommands()
                {
                    return
                    [
                        ..base.getCommands(),
                        new Command("new", "Record a new Test Case", newTestCase)
                    ];
                }

                protected abstract Recording record(string lang, string testCaseName);
            }

            public abstract record Recording(Recorder.State.Recording recorder): BaseRepl
            {
                enum CaptureState { Stopped, Recording, Paused }

                CaptureState captureState;

                protected override string prompt()
                {
                    return captureState switch
                    {
                        CaptureState.Recording => "[REC]> ",
                        CaptureState.Paused => "[PAUSED]> ",
                        _ => base.prompt()
                    };
                }

                void callKeyword(string input)
                {
                    if (captureState != CaptureState.Recording)
                    {
                        Console.WriteLine(captureState == CaptureState.Paused
                            ? "Recording is paused. Call `resume` to continue recording."
                            : "No recording in progress. Call `start` to start recording.");
                        return;
                    }

                    var keywordCall = Regex.Split(input, @"\s\s+");
                    if (keywordCall.Length < 2)
                    {
                        Console.WriteLine("Invalid keyword call. Please provide at least a return value and a keyword name.");
                        return;
                    }

                    var timestamp = Stopwatch.GetTimestamp();
                    recorder.addKeywordCall(timestamp, keywordCall);
                }

                void save()
                {
                    if (captureState != CaptureState.Stopped)
                    {
                        Console.WriteLine("Recording in progress. Call `stop` to stop recording before saving.");
                        return;
                    }

                    recorder.save();
                }

                protected override List<string> getBanner()
                {
                    return [
                        "Call `start` to start recording, `pause` and `resume` to suspend and continue, and `stop` to finish recording.",
                    ]; 
                }
                
                protected override List<Command> getCommands()
                {
                    return
                    [
                        ..base.getCommands(),
                        new Command("save", "Save the recording to the current .robot file", save),
                        new Command("start", "Start recording", start),
                        new Command("pause", "Pause recording", pause),
                        new Command("resume", "Resume a paused recording", resume),
                        new Command("stop", "Stop recording", stopRecording)
                    ];
                }

                protected override void handleCommand(string command)
                {
                    if (command.StartsWith('$'))
                    {
                        callKeyword(command);
                    }
                    else
                    {
                        base.handleCommand(command);
                    }
                }

                void start()
                {
                    if (captureState != CaptureState.Stopped)
                    {
                        Console.WriteLine(captureState == CaptureState.Paused
                            ? "Recording is paused. Call `resume` to continue recording."
                            : "Recording is already in progress.");
                        return;
                    }

                    recorder.start();
                    captureState = CaptureState.Recording;
                    Console.WriteLine("Recording started.");
                    Console.WriteLine("To manually call a keyword: ${return_value}   <keyword_name>   [<arg1>   <arg2> ...]");
                }

                void pause()
                {
                    if (captureState != CaptureState.Recording)
                    {
                        Console.WriteLine("Recording must be active to pause it.");
                        return;
                    }

                    recorder.pause();
                    captureState = CaptureState.Paused;
                    Console.WriteLine("Recording paused.");
                }

                void resume()
                {
                    if (captureState != CaptureState.Paused)
                    {
                        Console.WriteLine("Recording must be paused to resume it.");
                        return;
                    }

                    recorder.resume();
                    captureState = CaptureState.Recording;
                    Console.WriteLine("Recording resumed.");
                }

                void stopRecording()
                {
                    if (captureState == CaptureState.Stopped)
                    {
                        Console.WriteLine("No recording in progress.");
                        return;
                    }

                    recorder.stop();
                    captureState = CaptureState.Stopped;
                    Console.WriteLine("Recording stopped.");
                    onStopped();
                }

                protected virtual void onStopped() {}
            }
        }

        public static class Keyword
        {
            public static void start(bool debug)
            {
                new Idle(debug).repl();
            }

            public record Idle(bool debug) : State.Idle(new Recorder.Keyword.Initialized(debug))
            {
                protected override State.Recording record(string lang, string testCaseName)
                {
                    return new Recording(new Recorder.Keyword.Recording(debug, lang, testCaseName));
                }
            }

            public record Recording(Recorder.Keyword.Recording keywordRecorder): State.Recording(keywordRecorder)
            {
                protected override List<string> getBanner()
                {
                    return [
                        "Call `start` to start recording a keyword, `pause` and `resume` to suspend and continue, and `stop` to finish the keyword."
                    ]; 
                }

                protected override void onStopped()
                {
                    if (keywordRecorder.hasEvents())
                    {
                        var keywordName = validatedReadInput(
                            "Keyword name: ", 
                            keywordName => !string.IsNullOrWhiteSpace(keywordName), 
                            "Keyword name cannot be empty."
                        );
                        keywordRecorder.saveKeyword(keywordName);   
                        Console.WriteLine($"Keyword '{keywordName}' saved.");
                    }
                }
            }
        }

        public static class Robosapiens
        {
            public static void start(bool debug)
            {
                new Idle(debug).repl();
            }

            public record Idle(bool debug) : State.Idle(new Recorder.Robosapiens.Initialized(debug))
            {
                protected override State.Recording record(string lang, string testCaseName)
                {
                    return new Recording(new Recorder.Robosapiens.Recording(debug, lang, testCaseName));
                }
            }

            public record Recording(Recorder.Robosapiens.Recording robosapiensRecorder): State.Recording(robosapiensRecorder)
            {
            }
        }
    }
}