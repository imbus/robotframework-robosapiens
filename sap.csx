// VS Code: Disable C# Dev Kit, in the C# extension enable useOmniSharp
// dotnet tool install -g csharprepl
// csharprepl
// #r "nuget: YamlDotNet, 17.0.1"
// #load "sap.csx"
// using sapfewse;

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
session.Destroy += handleDestroy;

static string capitalize(string s)
{
    return char.ToUpper(s[0]) + s[1..];
}

record Event(string componentId, string componentType, string type, string name, List<object> values) {
    public string serialize()
    {
        var formatValue = (object val) => val switch
        {
            bool b => val.ToString().ToLower(),
            string s => $"\"{s}\"",
            _ => val.ToString()
        };
        var target = $"""(({componentType})session.FindById("{componentId}")).{name}""";

        return type switch
        {
            "Method" => target + "(" + string.Join(", ", values.Select(formatValue)) + ")",
            "Property" => target + " = " + formatValue(values[0]),
            _ => throw new Exception("Unknown event type")
        };
    }

    public override string ToString()
    {
        return $"Type: {type} | Name: {name} | Values: {string.Join(", ", values)}";
    }
}
var eventLog = new List<Event>();


void handleChange(GuiSession session, GuiComponent component, object commmandArray)
{
    var commands = (object[])commmandArray;
    var lastCommand = (object[])commands[^1];
    var type = lastCommand[0].ToString() switch {
        "M" => "Method",
        "SP" => "Property",
        _ => throw new Exception("Unknown type")
    };
    var name = capitalize(lastCommand[1].ToString());
    var values = lastCommand[2..].ToList();
    var componentType = component.Type switch
    {
        "GuiShell" => "Gui" + ((GuiShell)component).SubType switch
        {
            "DockShell" => "ContainerShell",
            string subtype => subtype
        },
        _ => component.Type
    };
    var e = new Event(component.Id, componentType, type, name, values);
    eventLog.Add(e);
    Console.WriteLine(component.Id);
    Console.WriteLine(component.Type);
    Console.WriteLine(e);

    if (!component.Type.EndsWith("Window"))
    {
        Console.WriteLine(JsonToYaml(getObjectTree(component.Id)));
    }
}

void handleDestroy(GuiSession session)
{
    recordStop();
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

void saveRecording(string filename)
{
    var preamble = """
    // dotnet tool install -g dotnet-script
    // dotnet script script.csx
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

    """;

    File.WriteAllText(
        Path.Combine(Directory.GetCurrentDirectory(), filename),
        preamble + string.Join(";" + Environment.NewLine, eventLog.Select(e => e.serialize()))
    );
}

void saveWindowTree()
{
    File.WriteAllText(
        Path.Combine(Directory.GetCurrentDirectory(), "sap.json"),
        getObjectTree(session.ActiveWindow.Id)
    );
}
