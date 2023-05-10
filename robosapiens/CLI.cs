using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SAPiens;

namespace RoboSAPiens
{
    public class CLI
    {
        public class Logger: SAPiens.ILogger {
            public void error(params string[] messages) {
                Console.Error.WriteLine(String.Join(Environment.NewLine, messages));
            }

            public void info(params string[] messages) {
                Console.WriteLine(String.Join(Environment.NewLine, messages));
            }
        }

        public record Arg(string name, object? default_value, string doc, Action<object> handler, bool export=true);

        public class Arguments {
            List<Arg> args;

            public Arguments(List<Arg> arguments) {
                this.args = arguments;
            }

            public Dictionary<string, Arg> asDict() {
                return args.ToDictionary(arg => arg.name, arg => arg);
            }

            public Dictionary<string, Dictionary<string, object>> asSpec() {
                return args
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
                    );
            }

            public override string ToString() 
            {
                return String.Join("\n",
                    args.Select(arg => $"--{arg.name} {getPlaceholder(arg.handler)}\n  {arg.doc}")
                        .ToList()
                    );
            }
        }

        public Arguments arguments;
        public ILogger logger;
        private Options options;

        public CLI() {
            // must be initialized before the arguments
            this.options = default(Options) with {port = 8270};
            this.arguments = new Arguments(new List<Arg>
            {
                new Arg("export-api",
                    null,
                    "Export the RoboSAPiens API specification in JSON format to the specified file and exit",
                    (filename) => exportApi((string)filename),
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
            });
            this.logger = new Logger();
        }

        private void exportApi(string fileName)
        {
            if (!Path.HasExtension(fileName)) fileName += ".json";
            var cwd = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(cwd, fileName);

            var api = new 
            {
                doc = new 
                {
                    intro = "This is the introduction at the beginning of the documentation",
                    init = "This is the section 'Importing' in the documentation"
                },
                args = arguments.asSpec(),
                keywords = SAPiens.KeywordLibrary.getKeywordSpecs(),
                specs = new {}
            };

            try
            {
                JSON.writeFile(filePath, JSON.serialize(api));
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                exitWithError(e.Message);
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

        public void exitWithError(string message) {
            logger.error(message);
            Environment.Exit(1);
        }

        public Options parseArgs(string[] args) 
        {
            var argsQueue = new Queue<string>(args);
            var argsDict = arguments.asDict();

            while (argsQueue.Count > 0) 
            {
                string option = argsQueue.Dequeue();
                var argName = option.Replace("--", "");

                if (!argsDict.ContainsKey(argName)) 
                    exitWithError(
                        $"The option `{option}` is invalid. " +
                        "Run RoboSAPiens.exe without arguments to see the list of valid options."
                    );

                var arg = argsDict[argName];

                if (arg.default_value is bool) arg.handler(true);
                else 
                {
                    if (argsQueue.Count > 0) arg.handler(argsQueue.Dequeue());
                    else exitWithError(
                        $"Missing argument '{getPlaceholder(arg.handler)}'. " +
                        $"Run RoboSAPiens.exe to see the documentation for the option {option}."
                    );
                }
            }

            return options;
        }
    }
}