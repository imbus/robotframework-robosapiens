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
using System.Text.Json;
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

static string capitalize(string s)
{
    return char.ToUpper(s[0]) + s[1..];
}

record Event(string window, string componentId, string componentType, Locator? locator, string type, string name, List<object> values) {
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
    Console.WriteLine("============");
    Console.WriteLine($"Id: {component.Id}");
    Console.WriteLine($"Type: {component.Type}");
    
    try
{
    var commands = (object[])commmandArray;
        var events = commands.Select(command => toEvent((object[])command, component)).ToList();
        eventLog.AddRange(events);
        Console.WriteLine("~~ Events ~~");
        events.ForEach(Console.WriteLine);
    }
    catch (System.Exception ex)
    {
        Console.WriteLine(ex);
    }

    Console.WriteLine("============");
}


record Locator(string hLabel, string? vLabel=null, string? contents=null)
{
    public override string ToString()
    {
        return $"{{ hLabel = {hLabel}, vlabel = {vLabel}, contents = {contents} }}";
    }
}

record CellLocator(string row, string col)
{
    public override string ToString()
    {
        return $"{{ row = {row}, col = {col} }}";
    }
}

string getTooltip(GuiVComponent component)
{
    if (component.DefaultTooltip != "")
    {
        return component.DefaultTooltip.Trim();
    }

    if (component.Tooltip != "")
    {
        return component.Tooltip.Trim();
    }

    return $"Component has no tooltip: {component.Id}";
}

string getButtonLabel(GuiButton button)
{
    if (button.Text != "")
    {
        return button.Text.Trim();
    }

    return button.Tooltip.Trim();
}

string getTabLabel(GuiTab tab)
{
    if (tab.Text != "")
    {
        return tab.Text.Trim();
    }

    return getTooltip((GuiVComponent)tab);
}

string getTextFieldLabel(GuiTextField textField)
{
    if (textField.LeftLabel != null)
    {
        return textField.LeftLabel.Text.Trim();
    }

    return getTooltip((GuiVComponent)textField);
}

Locator? getLocator(GuiVComponent component)
{
    return component switch
    {
        GuiButton button => new Locator(getButtonLabel(button)),
        GuiTab tab => new Locator(getTabLabel(tab)),
        GuiTextField textField => new Locator(getTextFieldLabel(textField)),
        _ => null
    };
}

public enum TreeType 
{
    Simple,
    List,
    Column
}

string getText(GuiTree tree, string nodeKey)
{
    var treeType = (TreeType)tree.GetTreeType();
    if (treeType == TreeType.List) 
    {
        var texts = new List<string>();
        for (int i = 1; i < tree.GetListTreeNodeItemCount(nodeKey)+1; i++)
        {
            var itemText = tree.GetItemText(nodeKey, i.ToString());
            if (itemText != null && itemText.Trim() != "") {
                texts.Add(itemText.Replace("/", "|"));
            }
        }
        return string.Join(" ", texts);
    }

    return tree.GetNodeTextByKey(nodeKey).Replace("/", "|");
}

string getParentPath(string path)
{
    var pathParts = path.Split("\\");
    var parent_path = string.Join("\\", pathParts[0..^1]);

    if (parent_path != "") {
        return parent_path;
    }

    return "ROOT";
}

string getTextPath(GuiTree guiTree, string path, string textPath)
{
    var parentPath = getParentPath(path);

    if (parentPath == "ROOT") {
        return textPath;
    }
    else {
        var parentText = getText(guiTree, guiTree.GetNodeKeyByPath(parentPath));
        return getTextPath(guiTree, parentPath, $"{parentText}/{textPath}");
    }
}

string getTreeElementPath(GuiTree tree, string nodeKey)
{
    var nodePath = tree.GetNodePathByKey(nodeKey);
    var text = getText(tree, nodeKey);
    return getTextPath(tree, nodePath, text);
}

Event toEvent(object[] command, GuiComponent component)
{
    var type = command[0].ToString() switch {
        "M" => "Method",
        "SP" => "Property",
        _ => throw new Exception("Unknown type")
    };
    var name = capitalize(command[1].ToString());
    var values = command[2..].ToList();
    var componentType = component.Type switch
    {
        "GuiDockShell" => "GuiContainerShell",
        "GuiShell" => "Gui" + ((GuiShell)component).SubType,
        _ => component.Type
    };
    var window = session.ActiveWindow.Text;
    var locator = componentType switch
    {
        "GuiTree" => name switch
        {
            "DoubleClickItem" => new Locator(getTreeElementPath((GuiTree)component, values[0].ToString())),
            _ => null
        },
        _ => getLocator((GuiVComponent)component)
    };

    return new Event(window, component.Id, componentType, locator, type, name, values);
    }


void handleDestroy(GuiSession session)
{
    recordStop();
}

void recordStart()
{
    session.Change += handleChange;
    session.Destroy += handleDestroy;
    session.Record = true;
}

void recordStop()
{
    session.Record = false;
    session.Change -= handleChange;
    session.Destroy -= handleDestroy;
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

void saveEventLog(string filename)
{
    var json = JsonSerializer.Serialize(eventLog);

    File.WriteAllText(
        Path.Combine(Directory.GetCurrentDirectory(), filename),
        JsonToYaml(json)    
    );
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
