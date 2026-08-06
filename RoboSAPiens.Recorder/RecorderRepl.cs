using System.Text;

namespace RoboSAPiens.Recorder
{
    public interface IRecorderRepl
    {
        void start();
    }

    public abstract class BaseRecorderRepl<R>: IRecorderRepl where R: IRecorder
    {
        protected bool debug;
        protected R recorder;
        
        public BaseRecorderRepl(bool debug, R recorder) 
        {
            this.debug = debug;
            this.recorder = recorder;
        }

        protected static string? readInput(string prompt)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.Write(prompt);
            return Console.ReadLine();
        }
        
        protected virtual void handleCommand(string command)
        {
            switch (command)
            {
                case "":
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                case "help":
                    Console.WriteLine("Available commands:");
                    Console.WriteLine("  start - Start recording");
                    Console.WriteLine("  stop  - Stop recording");
                    Console.WriteLine("  save  - Save the recorded test case to a .robot file");
                    Console.WriteLine("  exit  - Exit the program");
                    break;
                case "save":
                    string testcase = readInput("Test Case: ")!;
                    string language = readInput("Language [en, de]: ")!;
                    while (language != "en" && language != "de")
                    {
                        Console.WriteLine($"Invalid language: {language}");
                        language = readInput("Language [en, de]: ")!;
                    }
                    recorder.save(language, testcase);
                    break;
                case "start":
                    recorder.start();
                    break;
                case "stop":
                    stop();
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        public void start()
        {
            Console.WriteLine("=============== RoboSAPiens Recorder CLI ===============");
            Console.WriteLine(recorder.recordingMode.ToString());
            Console.WriteLine("Type `help` to get the list of available commands.");

            while (true)
            {
                handleCommand(readInput("> ")!);
            }
        }

        protected virtual void stop()
        {
            recorder.stop();
        }
    }

    public static class RecorderRepl
    {
        public class Keyword: BaseRecorderRepl<RobotRecorder.Keyword>
        {
            public Keyword(bool debug) : base(debug, new RobotRecorder.Keyword(debug)) {}

            protected override void stop()
            {
                recorder.stop();
                var keywordName = readInput("Keyword name: ")!;
                recorder.saveKeyword(keywordName);
            }
        }

        public class RoboSAPiens: BaseRecorderRepl<RobotRecorder.RoboSAPiens>
        {
            public RoboSAPiens(bool debug) : base(debug, new RobotRecorder.RoboSAPiens(debug)) {}
        }
    }
}
