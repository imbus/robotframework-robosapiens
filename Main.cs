using System;
using System.IO;
using System.Net;

namespace RoboSAPiens {
    class _ {
        static void error(params string[] messages) {
            Console.Error.WriteLine(String.Join(Environment.NewLine, messages));
        }

        static void info(params string[] messages) {
            Console.WriteLine(String.Join(Environment.NewLine, messages));
        }

        static byte[] loadDocumentation(string htmlFile) {
            var rfDocumentation = new byte[]{};
            try {
                var roboSAPiensPath = System.AppContext.BaseDirectory;
                rfDocumentation = File.ReadAllBytes(Path.Combine(roboSAPiensPath, htmlFile));
            } 
            catch (Exception e) {
                error("Die Dokumentation von RoboSAPiens konnte nicht geladen werden.",
                      $"Fehlermeldung: {e.Message}");
            }
            return rfDocumentation;
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

            if (rfDocumentation.Length > 0) {
                info($"- Die Dokumentation der verfügbaren Keywords befindet sich auf {serverAddress}/doc.");
            }
            if (options.debug) {
                info("========== DEBUG-Modus aktiv ==========");
            } else {
                info("- Zum Debuggen RoboSAPiens.exe --debug ausführen.");
            }

            while (httpListener.IsListening) {
                var context = httpListener.GetContext();
                var url = context.Request.Url;

                if (url == null) {
                    if (options.debug) {
                        error("Die Anfrage enthält keinen URL.");
                    }
                    continue;
                }

                var endpoint = String.Join("", url.Segments);
                if (endpoint == "/doc" && rfDocumentation.Length > 0) {
                    context.Response.ContentType = "text/html";
                    context.Response.OutputStream.Write(rfDocumentation, 0, rfDocumentation.Length);
                    context.Response.OutputStream.Close();
                }
                else {
                    robotRemote.ProcessRequest(context);
                }
            }
        }
    }
}
