using System;

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

            if (options.record) {
                REPL.Recorder.Start(options.debug);
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
