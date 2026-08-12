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

            switch(options.recordingMode)
            {
                case RecordingMode.Keyword:
                    Repl.Keyword.start(options.debug);
                    break;
                case RecordingMode.Robosapiens:
                    Repl.Robosapiens.start(options.debug);
                    break;
            };

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
