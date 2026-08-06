using System;
using RoboSAPiens.Recorder;

namespace RoboSAPiens 
{
    class _ 
    {
        [STAThread]
        public static void Main(string[] args) 
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            CLI cli = new();
            var options = cli.parseArgs(args);
            var keywordLibrary = new KeywordLibrary(options, cli.logger);

            if (options.recordingMode != null)
            {
                IRecorderRepl repl = options.recordingMode switch 
                {
                    RecordingMode.Keyword => new RecorderRepl.Keyword(options.debug),
                    RecordingMode.RoboSAPiens => new RecorderRepl.RoboSAPiens(options.debug),
                    _ => throw new NotImplementedException($"The recording mode {options.recordingMode} is not supported.")
                };
                repl.start();
            }

            if (options.debug)
            {
                REPL.Debug.start(keywordLibrary);
            }

            if (options.jsonRepl)
            {
                REPL.Json.start(keywordLibrary);
            }
        }
    }
}
