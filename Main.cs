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
                info($"Der RoboSAPiens Keywordserver läuft auf {serverAddress}. Zum Stoppen Strg+C drücken.",
                    "- Zur Verwendung eines anderen Port RoboSAPiens.exe -p [PORT] ausführen.");
            } catch (Exception e) {
                error("Der RoboSAPiens Keywordserver konnnte nicht gestartet werden.",
                      $"Fehlermeldung: {e.Message}");
                Environment.Exit(1);
            }
        }

        public static void Main(string[] args) {
            const string host = "http://127.0.0.1";
            var options = new Options(args);
            var serverAddress = $"{host}:{options.port}";
            var httpListener = new HttpListener();
            var robotRemote = new RobotRemote(options);

            startServer(httpListener, serverAddress);

            if (options.debug) {
                info("========== DEBUG-Modus aktiv ==========");
            } 
            else {
                info("- Zum Debuggen RoboSAPiens.exe --debug ausführen.");
            }

            while (httpListener.IsListening) {
                robotRemote.ProcessRequest(httpListener.GetContext());
            }
        }
    }
}
