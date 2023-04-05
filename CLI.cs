using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;

namespace RoboSAPiens
{
    public class CLI
    {
        public record struct Options(bool debug, int port, bool presenterMode);

        public static Options options = default(Options) with {port = 8270};

        public static void error(params string[] messages) {
            Console.Error.WriteLine(String.Join(Environment.NewLine, messages));
        }

        public static void info(params string[] messages) {
            Console.WriteLine(String.Join(Environment.NewLine, messages));
        }

        public class Commands
        {
            private static string cwd = AppDomain.CurrentDomain.BaseDirectory;                            

            public static void exportCli(string fileName)
            {
                var fileContent = 
                    arguments.Where(entry => entry.Value.export)
                            .Select(entry => new
                            {
                                name = entry.Key,
                                type = entry.Value.type.ToString(),
                                doc = entry.Value.doc
                            });

                JSON.SaveJsonFile(Path.Combine(cwd, fileName), fileContent);
                info($"CLI specification written to {fileName} in the current directory");
                Environment.Exit(0);
            }

            public static void exportApi(string fileName)
            {
                var methods = typeof(RoboSAPiens).GetMethods()
                    .Where(method => method.GetCustomAttribute(typeof(Keyword)) != null)
                    .Select(method => new 
                    {
                        name = method.Name,
                        args = method.GetParameters()
                                    .Select(param => new 
                                    {
                                        name = param.Name,
                                        type = param.ParameterType.ToString()
                                    })
                    });

                JSON.SaveJsonFile(Path.Combine(cwd, fileName), methods);
                info($"Keyword specification written to {fileName} in the current directory");
                Environment.Exit(0);
            }

            public static void help()
            {
                info("The following options are available:");
                arguments.Select(entry => $"--{entry.Key} {getPlaceholder(entry.Value.handler)}\n  {entry.Value.doc}")
                         .ToList()
                         .ForEach(line => info(line));
                
                Environment.Exit(0);
            }
        }

        private static String getPlaceholder(Action<object> action)
        {
            return action.Method.GetParameters().First().Name! switch
            {
                "_" => "",
                String name => name.ToUpper()
            };
        }

        private enum ArgType
        {
            Bool,
            Int,
            Str
        }

        private record Arg(ArgType type, string doc, bool export, Action<object> handler);

        private static Dictionary<string, Arg> arguments = new Dictionary<string, Arg>() 
        {
            {"debug", new Arg(
                ArgType.Bool,
                "Print detailed information to stdout when classifying and searching GUI elements",
                false,
                (_) => options = options with {debug = true}
            )},
            {"export-api", new Arg(
                ArgType.Str,
                "Export the Robot Framework API specification in JSON format to the specified file and exit",
                false,
                (filename) => Commands.exportApi((string)filename)
            )},
            {"export-cli", new Arg(
                ArgType.Str,
                "Export the CLI specification in JSON format to the specified file and exit",
                false,
                (filename) => Commands.exportCli((string)filename)
            )},
            {"port", new Arg(
                ArgType.Int,
                $"Set the port of the HTTP server implementing the Remote interface. (Default: {options.port})",
                true,
                (port) => options = options with {port = Int32.Parse((string)port)}
            )},
            {"presenter-mode", new Arg(
                ArgType.Bool,
                "Highlight each GUI element acted upon",
                true,
                (_) => options = options with {presenterMode = true}
            )}
        };

        public static void parseArgs(Queue<string> args) 
        {
            while (args.Count > 0) {
                string option = args.Dequeue();
                var argName = option.Replace("--", "");

                if (!arguments.ContainsKey(argName))
                {
                    error($"The option `{option}` is invalid. " +
                        "Run RoboSAPiens.exe without arguments to see the list of valid options.");
                    Environment.Exit(1);
                }

                var arg = arguments[argName];

                if (arg.type == ArgType.Bool) {
                    arg.handler(true);
                }
                else {
                    if (args.Count > 0) arg.handler(args.Dequeue());
                    else {
                        error($"Missing argument '{getPlaceholder(arg.handler)}'. " +
                            $"Run RoboSAPiens.exe to see the documentation for the option {option}.");
                        Environment.Exit(1);
                    }
                }
            }
        }
    }
}