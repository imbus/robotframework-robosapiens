using System.Diagnostics;
using System.Text;
using PhotinoNET;
using PhotinoNET.Server;

namespace Playground;

class Program
{
    public static string handleResponse(string response)
    {
        var jsonResponse = JSON.deserialize(response);

        if (jsonResponse != null) 
        {
            var error = jsonResponse.error;
            if (error != null)
            {
                var robotResult = error.data;
                return robotResult.error;
            }

            var result = jsonResponse.result;
            if (result != null)
            {
                return result.output;
            }
        }

        return "Received null";
    }

    [STAThread]
    public static void Main(string[] args)
    {
        var process = new Process();
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        process.StartInfo.FileName = "RoboSAPiens.exe";
        process.Start();


        PhotinoServer
            .CreateStaticFileServer(args, out string baseUrl)
            .RunAsync();

        // Window title declared here for visibility
        string windowTitle = "RoboSAPiens Playground";

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

                // var attachToRunningSap = @"{""jsonrpc"": ""2.0"",""id"": 1,""method"": ""AttachToRunningSap"",""params"": []}";
                process.StandardInput.WriteLine(message);

                StringBuilder sb = new StringBuilder();
                for(string? line; !String.IsNullOrEmpty(line = process.StandardOutput.ReadLine());)
                    sb.Append(line);

                var result = handleResponse(sb.ToString());
                window.SendWebMessage(result);

            })
            .Load($"{baseUrl}/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.

        window.WaitForClose();
    }
}
