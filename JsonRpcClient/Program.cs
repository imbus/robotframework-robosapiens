using System.Diagnostics;

string json = """
    {"jsonrpc": "2.0","id": 1,"method": "OpenSap","params": ["locator"]}
""";

using (Process p = new())
{
    p.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "RoboSAPiens.exe");
    p.StartInfo.RedirectStandardInput = true;
    p.StartInfo.RedirectStandardOutput = true;
    p.OutputDataReceived += (sender, @event) => Console.WriteLine(@event.Data);
    p.Start();
    StreamWriter sw = p.StandardInput;
    p.BeginOutputReadLine();
    sw.WriteLine(json);
    sw.WriteLine(json);
    sw.WriteLine("quit");
    p.WaitForExit();
}
