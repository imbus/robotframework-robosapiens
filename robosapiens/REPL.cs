using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using RoboSAPiens.Recorder;

namespace RoboSAPiens
{
    class REPL 
    {
        public static class Debug
        {
            public static string? readInput()
            {
                Console.InputEncoding = Encoding.Unicode;
                Console.Write("> ");
                return Console.ReadLine();
            }

            public static void start(KeywordLibrary keywordLibrary)
            {
                Console.WriteLine($"=============== DEBUG-Mode active ===============");
                Console.WriteLine("Type `help` to get the list of available keywords.");
                Console.WriteLine("Type `KeywordName  Args...` to call a keyword.");
                Console.WriteLine("Type `quit` to exit.");

                string? input;
                while ((input = readInput()) != null && input != "quit")
                {
                    try
                    {
                        if (input == "help")
                        {
                            keywordLibrary.getKeywordNames().ForEach(Console.WriteLine);
                        }
                        else
                        {
                            string[] tokenize(string input) => Regex.Split(input.Replace("\t", "  "), @"\s\s+");

                            (List<string>, Dictionary<string, object>) classifyParams(string[] @params, Dictionary<string, string> paramTypes)
                            {
                                var args = new List<string>();
                                var kwargs = new Dictionary<string, object>();

                                foreach (var param in @params)
                                {
                                    if (!param.Contains("=") || param.StartsWith("="))
                                    {
                                        args.Add(param);
                                    }
                                    else
                                    {
                                        var nameValue = param.Split("=");
                                        var name = nameValue[0].Trim();
                                        var value = nameValue[1].Trim();
                                        var type = paramTypes[name];

                                        if (type.Contains("System.Int32"))
                                        {
                                            kwargs.Add(name, int.Parse(value));
                                        }
                                        else if (type.Contains("System.Boolean"))
                                        {
                                            if (value.ToLower() == "true")
                                            {
                                                kwargs.Add(name, true);
                                            }

                                            if (value.ToLower() == "false")
                                            {
                                                kwargs.Add(name, false);
                                            }
                                        }
                                        else
                                        {
                                            kwargs.Add(name, value);
                                        }
                                    }
                                }

                                return (args, kwargs);
                            }

                            Dictionary<TKey, TValue> merge<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2) where TKey: notnull
                            {
                                var result = new Dictionary<TKey, TValue>();

                                foreach (var kvp in dict1)
                                {
                                    if (dict2.ContainsKey(kvp.Key))
                                    {
                                        result.Add(kvp.Key, dict2[kvp.Key]);
                                    }
                                    else
                                    {
                                        result.Add(kvp.Key, kvp.Value);
                                    }
                                }

                                return result;
                            }

                            RobotResult callKeyword(string method, (List<string>, Dictionary<string, object>) @params)
                            {
                                (var args, var kwargs) = @params;
                                var callKwargs = merge(keywordLibrary.getKeywordDefaultArguments(method), kwargs);

                                return keywordLibrary.callKeyword(method, args.ToArray(), callKwargs);
                            }

                            var result = tokenize(input.Trim()) switch
                            {
                                [var method] => callKeyword(method, (new List<string>(), new Dictionary<string, object>())),
                                [var method, .. var @params] => callKeyword(method, classifyParams(@params, keywordLibrary.getKeywordArgumentTypes(method))),
                                [] => null
                            };

                            if (result == null) {
                                Console.WriteLine("Invalid input. Expected: MethodName  args  kwargs, using at least two spaces as delimiter.");
                            }

                            var response = result!.status switch
                            {
                                Status.FAIL => result.error + Environment.NewLine + result.traceback,
                                Status.PASS => result.output,
                                _ => throw new NotImplementedException()
                            };
                            
                            Console.WriteLine();
                            Console.WriteLine(response);
                        }
                    }
                    catch (Exception e) 
                    {
                        Console.WriteLine();
                        Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                    }
                }
            }
        }

        public static class Json
        {
            public static string? readInput()
            {
                return Console.ReadLine();
            }

            public static void start(KeywordLibrary keywordLibrary)
            {
                string? input;
                while ((input = readInput()) != null && input != "quit")
                {
                    var request = JSON.deserialize(input) ?? throw new Exception("Received null");
                    var result = keywordLibrary.callKeyword(request.method, request.args, request.kwargs);
                    var response = result.status switch
                    {
                        Status.FAIL => JSON.Fail(new JSONError(-32000, "Keyword call failed.", result), id: request.id),
                        Status.PASS => JSON.Pass(result, id: request.id),
                        _ => throw new NotImplementedException()
                    };
                    Console.WriteLine(JSON.serialize(response, typeof(JSONResponse)));
                    Console.WriteLine();
                }
            }
        }

        public static class Recorder
        {
            static string? readInput(string prompt)
            {
                Console.InputEncoding = Encoding.Unicode;
                Console.Write(prompt);
                return Console.ReadLine();
            }

            public static void Start(bool debug)
            {
                Console.WriteLine($"=============== RoboSAPiens Recorder CLI ===============");
                Console.WriteLine("Type `help` to get the list of available commands.");
                Console.WriteLine("Type `quit` to exit.");

                GuiRecorder? recorder = null;

                string? input;
                while ((input = readInput("> ")) != null)
                {
                    try
                    {
                        switch (input)
                        {
                            case "help":
                                Console.WriteLine("Available commands:");
                                Console.WriteLine("  start - Start recording");
                                Console.WriteLine("  stop  - Stop recording");
                                Console.WriteLine("  save  - Save the recorded steps to a .robot file");
                                Console.WriteLine("  quit  - Exit the program");
                                break;
                            case "quit":
                                if (recorder != null)
                                {
                                    recorder.recordStop();
                                }
                                Environment.Exit(0);
                                break;
                            case "save":
                                if (recorder != null)
                                {
                                    string testcase = readInput("Test Case: ")!;
                                    string language = readInput("Language [en, de]: ")!;
                                    recorder.saveRobotFile(testcase, language.ToUpper());
                                    
                                    if (debug)
                                    {
                                        string filename = testcase.ToLower().Replace(" ", "-");
                                        recorder.saveEventLog(filename);
                                        recorder.saveKeyGuiLog(filename);
                                        recorder.saveKeywordLog(filename, language.ToUpper());
                                    }
                                }
                                break;
                            case "start":
                                if (recorder != null)
                                {
                                    recorder.recordStop();
                                }
                                recorder = new GuiRecorder(debug);
                                Console.WriteLine("Recording...");
                                recorder.recordStart();
                                break;
                            case "stop":
                                if (recorder != null)
                                {
                                    recorder.recordStop();
                                    Console.WriteLine("Recording stopped.");
                                }
                                break;
                            default:
                                Console.WriteLine($"Unknown command: {input}");
                                break;
                        }
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
    }
}