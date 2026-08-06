using System;
using System.Collections.Generic;
using System.Linq;
using RoboSAPiens.Recorder;

namespace RoboSAPiens
{
    public record struct Options(bool debug, bool jsonRepl, bool presenterMode, IRecordingMode? recordingMode);

    public class CLI
    {
        public class Logger: ILogger
        {
            public void error(params string[] messages)
            {
                Console.Error.WriteLine(string.Join(Environment.NewLine, messages));
            }

            public void info(params string[] messages)
            {
                Console.WriteLine(string.Join(Environment.NewLine, messages));
            }
        }

        public record Arg(string name, string doc)
        {
            public string cliDocumentation()
            {
                return $"--{name}\n  {doc}";
            }
        }

        public List<Arg> arguments =
        [
            new Arg(
                "debug",
                "Start the debugging REPL (development only)."
                ),
            new Arg(
                "json-repl",
                "Start the JSON REPL (used by the Robot Framework libraries)."
                ),
            new Arg(
                "presenter-mode",
                "Highlight each GUI element acted upon."
                ),
            new Arg(
                "record",
                "Record the actions performed in the SAP GUI and save them to a .robot file."
            ),
            new Arg(
                "record-keywords",
                "Record keywords and save them to a .robot file."
            )
        ];
        public ILogger logger = new Logger();
        public void help()
        {
            logger.info("RoboSAPiens :: SAP GUI automation for humans");
            logger.info("Usage: RoboSAPiens.exe --OPTION ...");
            logger.info("The following options are available:");
            logger.info(string.Join("\n", arguments.Select(arg => arg.cliDocumentation())));
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