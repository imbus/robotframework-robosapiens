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
            if (args.Length == 0)
            {
                help();
                Environment.Exit(0);
            }

            var argNames = arguments.Select(arg => arg.name).ToHashSet();
            var flags = 
                args
                .Where(arg => arg.StartsWith("--"))
                .Select(arg => (arg.Replace("--", ""), true))
                .ToDictionary();

            var invalidFlag = flags.Keys.FirstOrDefault(flag => !argNames.Contains(flag));
            if (invalidFlag != null)
            {
                logger.error($"The option `--{invalidFlag}` is invalid.");
                Environment.Exit(1);
            }

            return new Options(
                debug         : flags.GetValueOrDefault("debug"),
                jsonRepl      : flags.GetValueOrDefault("json-repl"),
                presenterMode : flags.GetValueOrDefault("presenter-mode"),
                recordingMode : flags.GetValueOrDefault("record")? new RecordingMode.RoboSAPiens() : 
                                flags.GetValueOrDefault("record-keywords")? new RecordingMode.Keyword() : 
                                null
            );
        }
    }
}