using System;

namespace RoboSAPiens 
{
    class _ 
    {
        [STAThread]
        public static void Main(string[] args) 
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            CLI cli = new CLI();
            var options = cli.parseArgs(args);
            var keywordLibrary = new KeywordLibrary(options, cli.logger);

            if (options.debug) {
                REPL.Debug.start(keywordLibrary);
            }
            else {
                REPL.Json.start(keywordLibrary);
            }
        }
    }
}
