using System;

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
            string? jsonString;

            while ((jsonString = Console.ReadLine()) != null && jsonString != "quit")
            {
                try
                {
                    var request = JSON.deserialize(jsonString);
                    switch(request)
                    {
                        case JSONRequest:
                            var result = keywordLibrary.callKeyword(request.method, request.@params);
                            if (result.status == RobotResult.FAIL)
                            {
                                var response = JSON.Fail(new JSONError(-32000, "Keyword call failed.", result), id: request.id);
                                Console.WriteLine(JSON.serialize(response, typeof(JSONResponse)));
                                Console.WriteLine();
                            }
                            if (result.status == RobotResult.PASS)
                            {
                                var response = JSON.Pass(result, id: request.id);
                                Console.WriteLine(JSON.serialize(response, typeof(JSONResponse)));
                                Console.WriteLine();
                            }
                            break;
                        default:
                            if (options.debug) cli.logger.error("Received null");
                            break;
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
