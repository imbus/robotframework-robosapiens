using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoboSAPiens
{
    class REPL 
    {
        public static class Debug
        {
            public static string? readInput()
            {
                Console.Write("> ");
                return Console.ReadLine();
            }

            static object[] parseArgs(string[] args, string[] types)
            {
                return args.Zip(types)
                    .Select<(string, string), object>(argType => 
                        argType switch {
                            (var arg, var type) when type.Contains("System.Int32") => int.Parse(arg),
                            (var arg, _) => arg
                        }
                    )
                    .ToArray();
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
                            var result = Regex.Split(input, @"\s\s+") switch {
                                [] => null,
                                [var method, ..var args] => keywordLibrary.callKeyword(
                                    method, 
                                    [.. parseArgs(args, keywordLibrary.getKeywordArgumentTypes(method)), .. keywordLibrary.getKeywordDefaultArguments(method)]
                                ),
                            };

                            if (result == null) {
                                Console.WriteLine("Invalid input. Expected: MethodName  Args... using at least two spaces as delimiter.");
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
                    var result = keywordLibrary.callKeyword(request.method, request.args);
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
    }
}