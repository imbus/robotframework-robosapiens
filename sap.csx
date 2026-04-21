// dotnet tool install -g csharprepl
// #load "sap.csx"

#r "robosapiens/lib/sapfewse.dll"
#r "robosapiens/lib/saprotwr.net.dll"

using System.Reflection;
using sapfewse;
using saprotwr.net;

var rot = new CSapROTWrapper();
var sapGui = rot.GetROTEntry("SAPGUI");
var sap = (GuiApplication)sapGui.GetType().InvokeMember(
    "GetScriptingEngine",
    BindingFlags.InvokeMethod,
    null,
    sapGui,
    null
);

var connection = (GuiConnection)sap.Connections.ElementAt(0);
var session = (GuiSession)connection.Sessions.ElementAt(0);

var getWindowTree = (GuiSession session) => session.GetObjectTree(
    session.ActiveWindow.Id,
    new string[] { "Id", "Top", "Left", "Width", "Height", "Text", "Type" }
).Replace("\\", "");

var saveWindowTree = (GuiSession session) => File.WriteAllText(
    Path.Combine(Directory.GetCurrentDirectory(), "sap.json"),
    getWindowTree(session)
);
