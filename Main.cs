using System;
using System.Net;

namespace RoboSAPiens {
    class _ {
        static void startServer(HttpListener listener, string host, int port) 
        {
            var serverAddress = $"http://{host}:{port}";
            listener.Prefixes.Add($"{serverAddress}/");

            try 
            {
                listener.Start();
                CLI.banner();
                CLI.info($"The RoboSAPiens keyword server runs on {serverAddress}. Press Ctrl+C to stop it.");
            }
            catch (HttpListenerException e) 
            {
                CLI.error("The RoboSAPiens keyword server could not be started.");
                if (e.ErrorCode == 32) {
                    CLI.error($"The port {port} is already in use.");
                }
                else {
                    CLI.error($"Error message: {e.Message}");
                }
                Environment.Exit(1);
            }
        }


        public static void Main(string[] args) 
        {           
            if (args.Length == 0) CLI.Commands.help();

            var options = CLI.parseArgs(args);
            const string host = "127.0.0.1";
            var httpListener = new HttpListener();
            var robotRemote = new RobotRemote(options);

            startServer(httpListener, host, options.port);

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
