using System;
using System.Net;

namespace RoboSAPiens 
{
    class _ 
    {
        const string banner = "RoboSAPiens :: SAP GUI automation for humans";
        const string fence = "=====" + "=====";

        record ServerAddress(string host, int port) 
        {
            public override string ToString() {
                return $"http://{host}:{port}";
            }
        };

        record EitherIO 
        {
            public record Ok(): EitherIO;
            public record Err(string message, string stacktrace = ""): EitherIO;
        }

        static EitherIO startServer(HttpListener listener, ServerAddress serverAddress) 
        {
            listener.Prefixes.Add(serverAddress + "/");

            try 
            {
                listener.Start();
                return new EitherIO.Ok();
            }
            catch (HttpListenerException e) 
            {
                if (e.ErrorCode == 32)
                    return new EitherIO.Err($"The port {serverAddress.port} is already in use");
                else 
                    return new EitherIO.Err($"Error: {e.Message}");
            }
        }

        public static void Main(string[] args) 
        {
            CLI cli = new CLI();
            
            if (args.Length == 0) 
            {
                cli.logger.info(banner);
                cli.logger.info("Usage: RoboSAPiens.exe --OPTION [ARG] ...");
                cli.logger.info("The following options are available:");
                cli.logger.info(cli.arguments.ToString());
                Environment.Exit(0);
            }

            var options = cli.parseArgs(args);

            const string host = "127.0.0.1";
            var serverAddress = new ServerAddress(host, options.port);
            var httpListener = new HttpListener();
            var robotRemote = new RobotRemote(options, cli.logger);

            switch (startServer(httpListener, serverAddress)) 
            {
                case EitherIO.Ok:
                    cli.logger.info(banner);
                    cli.logger.info($"The RoboSAPiens keyword server runs on {serverAddress}. Press Ctrl+C to stop it.");
                    if (options.debug) cli.logger.info($"{fence} DEBUG-Mode active {fence}");
                    break;

                case EitherIO.Err error:
                    cli.exitWithError($"The RoboSAPiens keyword server could not be started. {error.message}");
                    break;
            }

            while (httpListener.IsListening)
                robotRemote.ProcessRequest(httpListener.GetContext());
        }
    }
}
