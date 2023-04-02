using System;
using System.Net;

namespace RoboSAPiens {
    class _ {
        static void error(params string[] messages) {
            Console.Error.WriteLine(String.Join(Environment.NewLine, messages));
        }

        static void info(params string[] messages) {
            Console.WriteLine(String.Join(Environment.NewLine, messages));
        }

        static void startServer(HttpListener listener, string serverAddress) {
            listener.Prefixes.Add($"{serverAddress}/");

            try {
                listener.Start();
                info($"The RoboSAPiens keyword server runs on {serverAddress}. Press Ctrl+C to stop.");
            } catch (Exception e) {
                error("The RoboSAPiens keyword server could not be started.",
                      $"Error message: {e.Message}");
                Environment.Exit(1);
            }
        }

        public static void Main(string[] args) {
            System.Console.WriteLine("RoboSAPiens: SAP GUI automation for humans\n");
            
            if (args.Length == 0) {
                info("Usage: RoboSAPiens.exe --OPTION [ARG] ...\n");
                Config.Commands.help();
            }

            const string host = "http://127.0.0.1";
            Config.parseArgs(args);
            var options = Config.options;
            var serverAddress = $"{host}:{options.port}";
            var httpListener = new HttpListener();
            var robotRemote = new RobotRemote(options);

            startServer(httpListener, serverAddress);

            if (options.debug) {
                info("========== DEBUG-Mode active ==========");
            } 

            while (httpListener.IsListening) {
                robotRemote.ProcessRequest(httpListener.GetContext());
            }
        }
    }
}
