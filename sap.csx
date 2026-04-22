// VS Code: Disable C# Dev Kit, in the C# extension enable useOmniSharp
// dotnet tool install -g csharprepl
// csharprepl
// #r "nuget: YamlDotNet, 17.0.1"
// #r "robosapiens/lib/saprotwr.net.dll"
// #r "robosapiens/lib/sapfewse.dll"
// #load "sap.csx"

#r "robosapiens/lib/sapfewse.dll"
#r "robosapiens/lib/saprotwr.net.dll"
#r "nuget: YamlDotNet, 17.0.1"

using System.Reflection;
using System.Text.Json.Nodes;
using sapfewse;
using saprotwr.net;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


object? ConvertJsonNode(JsonNode? node)
{
    return node switch
    {
        JsonValue value => value.GetValue<object>().ToString(),
        JsonArray array => array.Select(ConvertJsonNode).ToList(),
        JsonObject obj => obj.ToDictionary(pair => pair.Key, pair => ConvertJsonNode(pair.Value)),
        _ => null
    };
}

string JsonToYaml(string json)
{
    var jsonNode = JsonNode.Parse(json) ?? throw new InvalidOperationException("Invalid JSON");
    var obj = ConvertJsonNode(jsonNode);

    var serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    return serializer.Serialize(obj);
}

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

record Event(string componentId, string name, List<object> values, string type) {
    public override string ToString()
    {
        return $" Name: {name} | Values: {string.Join(", ", values)} | Type: {type}";
    }
}
var eventLog = new List<Event>();


void handleChange(GuiSession session, GuiComponent component, object commmandArray)
{
    var commands = (object[])commmandArray;
    var lastCommand = (object[])commands[^1];
    var name = lastCommand[1].ToString();
    var values = lastCommand[2..].ToList();
    var type = lastCommand[0].ToString();
    var e = new Event(component.Id, name, values, type);
    eventLog.Add(e);
    Console.WriteLine(e);

    if (component.Type.EndsWith("Window"))
    {
        Console.WriteLine(component.Id);
    }
    else
    {
        Console.WriteLine(JsonToYaml(getObjectTree(component.Id)));
    }
}

void recordStart()
{
    session.Change += handleChange;
    session.Record = true;
}

void recordStop()
{
    session.Record = false;
    session.Change -= handleChange;
}

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
