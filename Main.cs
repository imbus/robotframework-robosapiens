using System;
using System.Net;

namespace RoboSAPiens {
    class _ {
        static void startServer(HttpListener listener, string serverAddress) {
            listener.Prefixes.Add($"{serverAddress}/");

            try {
                listener.Start();
                CLI.info($"The RoboSAPiens keyword server runs on {serverAddress}. Press Ctrl+C to stop.");
            } catch (Exception e) {
                CLI.error("The RoboSAPiens keyword server could not be started.",
                      $"Error message: {e.Message}");
                Environment.Exit(1);
            }
        }

        public static void Main(string[] args) {
            CLI.info("RoboSAPiens :: SAP GUI automation for humans");
            
            if (args.Length == 0) {
                CLI.info("Usage: RoboSAPiens.exe --OPTION [ARG] ...");
                CLI.Commands.help();
            }

            const string host = "http://127.0.0.1";
            var options = CLI.parseArgs(args);
            var serverAddress = $"{host}:{options.port}";
            var httpListener = new HttpListener();
            var robotRemote = new RobotRemote(options);

            startServer(httpListener, serverAddress);

            if (options.debug) {
                var fence = new string('=', 10);
                CLI.info($"{fence} DEBUG-Mode active {fence}");
            } 

            while (httpListener.IsListening) {
                robotRemote.ProcessRequest(httpListener.GetContext());
            }
        }
    }
}
