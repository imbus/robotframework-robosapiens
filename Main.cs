using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace RoboSAPiens {
    class _ {
        static void error(params string[] messages) {
            Console.Error.WriteLine(String.Join(Environment.NewLine, messages));
        }

        static void info(params string[] messages) {
            Console.WriteLine(String.Join(Environment.NewLine, messages));
        }

        static Stream loadDocumentation(string htmlFile) {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("RoboSAPiens.RoboSAPiens.html")!;
        }

        static void showDocumentation(HttpListenerContext context, Stream rfDocumentation) {
            context.Response.ContentType = "text/html";
            rfDocumentation.CopyTo(context.Response.OutputStream);
            context.Response.OutputStream.Close();
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
            const string docFile = "RoboSAPiens.html";
            var options = new Options(args);
            var serverAddress = $"{host}:{options.port}";
            var rfDocumentation = loadDocumentation(docFile);
            var httpListener = new HttpListener();
            var robotRemote = new RobotRemote(options);

            startServer(httpListener, serverAddress);

            info($"- Die Dokumentation der verfügbaren Keywords befindet sich auf {serverAddress}/doc.");
            if (options.debug) {
                info("========== DEBUG-Modus aktiv ==========");
            } else {
                info("- Zum Debuggen RoboSAPiens.exe --debug ausführen.");
            }

            while (httpListener.IsListening) {
                var context = httpListener.GetContext();
                Uri url = context.Request.Url!;

                var endpoint = String.Join("", url.Segments);
                switch (endpoint) {
                    case "/doc":
                        showDocumentation(context, rfDocumentation);
                        break;
                    default:
                        robotRemote.ProcessRequest(context);
                        break;
                };
            }
        }
    }
}
