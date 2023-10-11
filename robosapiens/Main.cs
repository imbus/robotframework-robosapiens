using System;
using System.Text.RegularExpressions;

namespace RoboSAPiens 
{
    class _ 
    {
        public static void Main(string[] args) 
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            CLI cli = new CLI();
            var options = cli.parseArgs(args);
            if (options.debug) cli.logger.info($"===== DEBUG-Mode active =====");
            var keywordLibrary = new KeywordLibrary(options, cli.logger);
            string? input;

            while ((input = Console.ReadLine()) != null && input != "quit")
            {
                try
                {
                    if (options.debug)
                    {
                        var result = Regex.Split(input, @"\s\s+") switch {
                            [] => null,
                            [var method, ..var @params] => keywordLibrary.callKeyword(method, @params),
                        };

                        if (result == null) {
                            Console.WriteLine("Invalid input. Expected: MethodName  Args... using at least two spaces as delimiter.");
                        }

                        var response = result!.status switch
                        {
                            Status.FAIL => result.error,
                            Status.PASS => result.output,
                            _ => throw new NotImplementedException()
                        };

                        Console.WriteLine();
                        Console.WriteLine("> " + response);
                    }
                    else
                    {
                        var request = JSON.deserialize(input) ?? throw new Exception("Received null");
                        var result = keywordLibrary.callKeyword(request.method, request.@params);
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
                catch (Exception e) 
                {
                    if (options.debug) cli.logger.error(e.Message, e.StackTrace ?? "");
                }
            }
        }
    }
}
