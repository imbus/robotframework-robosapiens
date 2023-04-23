using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RoboSAPiens
{
    public class CLI
    {
        public record struct Options(bool debug, int port, bool presenterMode);

        private static Options options = default(Options) with {port = 8270};

        public static void banner() {
            info("RoboSAPiens :: SAP GUI automation for humans");
        }

        public static void error(params string[] messages) {
            Console.Error.WriteLine(String.Join(Environment.NewLine, messages));
        }

        public static void info(params string[] messages) {
            Console.WriteLine(String.Join(Environment.NewLine, messages));
        }

        public class Commands
        {
            public static void exportApi(string fileName)
            {
                var api = new {
                    doc = new {
                        intro = "This is the introduction at the beginning of the documentation",
                        init = "This is the section 'Importing' in the documentation"
                    },
                    args = CLI.arguments
                            .Where(arg => arg.export)
                            .ToDictionary(
                                arg => arg.name, 
                                // We use a dictionary because an object cannot have a field called default (reserved keyword)
                                arg => new Dictionary<string, object>()
                                {
                                    { "name", arg.name },
                                    { "default", arg.default_value! },
                                    { "doc", arg.doc }
                                }
                        ),
                    keywords = RoboSAPiens.getKeywordSpecs(),
                    specs = new {}
                };

                var cwd = AppDomain.CurrentDomain.BaseDirectory;
                JSON.SaveJsonFile(Path.Combine(cwd, fileName), api);
                info($"RoboSAPiens specification written to {fileName} in the current directory");
                Environment.Exit(0);
            }

            public static void help()
            {
                var genDoc = (Arg arg) => $"--{arg.name} {getPlaceholder(arg.handler)}\n  {arg.doc}";

                banner();
                info("Usage: RoboSAPiens.exe --OPTION [ARG] ...");
                info("The following options are available:");
                arguments.Select(genDoc).ToList().ForEach(doc => info(doc));
                
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

        private record Arg(string name, object? default_value, string doc, Action<object> handler, bool export=true);

        private static List<Arg> arguments = new List<Arg>() 
        {
            new Arg("export-api",
                null,
                "Export the RoboSAPiens API specification in JSON format to the specified file and exit",
                (filename) => Commands.exportApi((string)filename),
                false
            ),
            new Arg("debug",
                options.debug,
                "Print detailed information to stdout when classifying and searching GUI elements",
                (_) => options = options with {debug = true},
                false
            ),
            new Arg("port",
                options.port,
                $"Set the port of the HTTP server implementing the Remote interface. (Default: {options.port})",
                (port) => options = options with {port = Int32.Parse((string)port)}
            ),
            new Arg("presenter-mode",
                options.presenterMode,
                "Highlight each GUI element acted upon",
                (_) => options = options with {presenterMode = true}
            )
        };

        public static Options parseArgs(string[] args) 
        {
            var argsDict = arguments.ToDictionary(arg => arg.name, arg => arg);
            var argsQueue = new Queue<string>(args);

            while (argsQueue.Count > 0) 
            {
                string option = argsQueue.Dequeue();
                var argName = option.Replace("--", "");

                if (!argsDict.ContainsKey(argName))
                {
                    error($"The option `{option}` is invalid. " +
                          "Run RoboSAPiens.exe without arguments to see the list of valid options.");
                    Environment.Exit(1);
                }

                var arg = argsDict[argName];

                if (arg.default_value is bool) arg.handler(true);
                else 
                {
                    if (argsQueue.Count > 0) arg.handler(argsQueue.Dequeue());
                    else 
                    {
                        error($"Missing argument '{getPlaceholder(arg.handler)}'. " +
                              $"Run RoboSAPiens.exe to see the documentation for the option {option}.");
                        Environment.Exit(1);
                    }
                }
            }

            return options;
        }
    }
}