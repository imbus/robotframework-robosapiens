// VS Code: Disable C# Dev Kit, in the C# extension enable useOmniSharp
// dotnet tool install -g csharprepl
// csharprepl
// #r "robosapiens/lib/saprotwr.net.dll"
// #r "robosapiens/lib/sapfewse.dll"
// #load "sap.csx"

#r "robosapiens/lib/sapfewse.dll"
#r "robosapiens/lib/saprotwr.net.dll"

using System.Reflection;
using sapfewse;
using saprotwr.net;


GuiSession getSession()
{
    var rot = new CSapROTWrapper();
    var sapGui = rot.GetROTEntry("SAPGUI") ?? throw new Exception("SAP Logon is not running.");
    var sap = (GuiApplication)sapGui.GetType().InvokeMember(
        "GetScriptingEngine",
        BindingFlags.InvokeMethod,
        null,
        sapGui,
        null
    );
    try
    {
    var connection = (GuiConnection)sap.Connections.ElementAt(0);
    return (GuiSession)connection.Sessions.ElementAt(0);
    }
    catch (Exception)
    {
        throw new Exception("Not connected to any SAP system.");
    }
}

var session = getSession();

void refresh()
{
    session = getSession();
}

string getObjectTree(string componentId)
{
    return session.GetObjectTree(
        componentId,
        new string[] { "Id", "Type", "SubType", "Top", "Left", "Width", "Height", "Text", "Tooltip"}
).Replace("\\", "");
}

var saveWindowTree = () => File.WriteAllText(
    Path.Combine(Directory.GetCurrentDirectory(), "sap.json"),
    getObjectTree(session.ActiveWindow.Id)
);
