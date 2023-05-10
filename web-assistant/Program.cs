﻿using System.Text;
using PhotinoNET;
using PhotinoNET.Server;
using Horizon.XmlRpc.Client;

namespace WebAssistant;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        PhotinoServer
            .CreateStaticFileServer(args, out string baseUrl)
            .RunAsync();

        var proxy = XmlRpcProxyGen.Create<IRobotProxy>();
        proxy.Url = "http://127.0.0.1:8270";

        // Window title declared here for visibility
        string windowTitle = "RoboSAPiens Web Assistant";

        // Creating a new PhotinoWindow instance with the fluent API
        var window = new PhotinoWindow()
            .SetTitle(windowTitle)
            // Resize to a percentage of the main monitor work area
            //.Resize(50, 50, "%")
            // Center window in the middle of the screen
            .Center()
            // Users can resize windows by default.
            // Let's make this one fixed instead.
            .SetResizable(false)
            .RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
            {
                contentType = "text/javascript";
                return new MemoryStream(Encoding.UTF8.GetBytes(@"
                        // (() =>{
                        //     window.setTimeout(() => {
                        //         alert(`🎉 Dynamically inserted JavaScript.`);
                        //     }, 1000);
                        // })();
                    "));
            })
            // Most event handlers can be registered after the
            // PhotinoWindow was instantiated by calling a registration 
            // method like the following RegisterWebMessageReceivedHandler.
            // This could be added in the PhotinoWindowOptions if preferred.
            .RegisterWebMessageReceivedHandler((object sender, string message) =>
            {
                var window = (PhotinoWindow)sender;
                var result = proxy.runKeyword("AttachToRunningSAP", new String[]{});
                result = proxy.runKeyword(message, new String[]{"Benutzer", "Student001"});

                // The message argument is coming in from sendMessage.
                // "window.external.sendMessage(message: string)"
                string response = $"Received message: \"{message}\"";

                // Send a message back the to JavaScript event handler.
                // "window.external.receiveMessage(callback: Function)"
                window.SendWebMessage((string)result["output"]);
            })
            .Load($"{baseUrl}/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.

        window.WaitForClose();
    }
}
