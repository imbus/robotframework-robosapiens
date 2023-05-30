using System;
using System.Collections.Generic;
using System.Linq;

namespace RoboSAPiens
{
    public class CLI
    {
        public class Logger: RoboSAPiens.ILogger {
            public void error(params string[] messages) {
                Console.Error.WriteLine(String.Join(Environment.NewLine, messages));
            }

            public void info(params string[] messages) {
                Console.WriteLine(String.Join(Environment.NewLine, messages));
            }
        }

        public record Arg(string name, bool default_value, string doc, Action enable, bool export=true);

        public class Arguments {
            List<Arg> args;

            public Arguments(List<Arg> arguments) {
                this.args = arguments;
            }

            public Dictionary<string, Arg> asDict() {
                return args.ToDictionary(arg => arg.name, arg => arg);
            }

            public List<Arg> get() {
                return args.Where(arg => arg.export).ToList();
            }

            public override string ToString() 
            {
                return String.Join("\n",
                    args.Select(arg => $"--{arg.name}\n  {arg.doc}")
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
                new Arg("debug",
                    options.debug,
                    "Print detailed information to stdout when classifying and searching GUI elements",
                    () => options = options with {debug = true},
                    false
                ),
                new Arg("help",
                    false,
                    "Print the available options",
                    () => help(),
                    false
                ),
                new Arg("presenter-mode",
                    options.presenterMode,
                    "Highlight each GUI element acted upon",
                    () => options = options with {presenterMode = true}
                )
            });
            this.logger = new Logger();
        }

        public void exitWithError(string message) {
            logger.error(message);
            Environment.Exit(1);
        }

        public void help() {
            const string banner = "RoboSAPiens :: SAP GUI automation for humans";
            logger.info(banner);
            logger.info("Usage: RoboSAPiens.exe --OPTION ...");
            logger.info("The following options are available:");
            logger.info(arguments.ToString());
            Environment.Exit(0);
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
                        "Run RoboSAPiens.exe --help to see the list of valid options."
                    );

                argsDict[argName].enable();
            }

            return options;
        }
    }
}