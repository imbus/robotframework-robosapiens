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
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using sapfewse;
using saprotwr.net;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

/// <summary>
/// Convert an empty string to null
/// </summary>
public static string NullIfEmpty(this string s)
{
    return string.IsNullOrEmpty(s) ? null : s;
}

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

        var keyGuiEvent = toKeyGuiEvent(events);
        if (keyGuiEvent != null) keyGuiEventLog.Add(keyGuiEvent);
    }
    catch (System.Exception ex)
    {
        Console.WriteLine(ex);
    }

    Console.WriteLine("============");
}


record Locator(string? hLabel=null, string? vLabel=null, string? contents=null, string? row=null, string? col=null)
{
    public override string ToString()
    {
        return (hLabel, vLabel, contents, row, col) switch
        {
            (string hLabel, null, null, null, null) => hLabel,
            (string hLabel, string vLabel, null, null, null) => $"{hLabel} @ {vLabel}",
            (null, string vLabel, null, null, null) => $"@ {vLabel}",
            (null, null, string contents, null, null) => $"= {contents}",
            (null, null, null, string row, string col) => $"{row}    {col}",
            _ => throw new Exception("Invalid locator")
        };
    }
}

string getTooltip(GuiVComponent component)
{
    return component.DefaultTooltip.Trim().NullIfEmpty() ??
           component.Tooltip.Trim();
}

string getButtonLabel(GuiButton button)
{
    var label = Regex.Replace(
        button.Text.Trim().NullIfEmpty() ?? button.Tooltip.Trim(), 
        @"\s\s+.*", ""
    );

    if (int.TryParse(label, out int _))
    {
        return $"\"{label}\"";
    }

    return label;
}

string getCheckBoxLabel(GuiCheckBox checkBox)
{
    return checkBox.Text.Trim().NullIfEmpty() ??
           checkBox.Tooltip.Trim().NullIfEmpty() ??
           checkBox.DefaultTooltip.Trim().NullIfEmpty() ??
           checkBox?.LeftLabel.Text.Trim() ??
           checkBox?.RightLabel.Text.Trim();
}

string getComboBoxLabel(GuiComboBox comboBox)
{
    return comboBox.AccTooltip.Trim().NullIfEmpty() ??
           comboBox.DefaultTooltip.Trim().NullIfEmpty() ??
           comboBox.Tooltip.Trim().NullIfEmpty() ??
           comboBox?.LeftLabel.Text.Trim() ??
           comboBox?.RightLabel.Text.Trim();
}

string getRadioButtonLabel(GuiRadioButton radioButton)
{
    return radioButton.Text.Trim().NullIfEmpty() ??
           radioButton.Tooltip.Trim().NullIfEmpty() ??
           radioButton.DefaultTooltip.Trim().NullIfEmpty() ??
           radioButton.LeftLabel?.Text.Trim()??
           radioButton.RightLabel?.Text.Trim();
}

string getTabLabel(GuiTab tab)
{
    return tab.Text.Trim().NullIfEmpty() ??
           getTooltip((GuiVComponent)tab);
}

string getTextFieldLabel(GuiTextField textField)
{
    // TODO: the closest label ?? tooltip
    return getTooltip((GuiVComponent)textField).NullIfEmpty() ?? textField.LeftLabel?.Text.Trim();
}

Locator? getTableCellLocator(GuiTableControl table, string componentId)
{
    for (int rowIndex0 = 0; rowIndex0 <= table.RowCount; rowIndex0++)
    {
        for (int colIdx = 0; colIdx < table.Columns.Length; colIdx++) 
        {
            try
            {
                var cell = table.GetCell(rowIndex0, colIdx);
                if (cell.Id == componentId)
                {
                    var column = (GuiTableColumn)table.Columns.ElementAt(colIdx);
                    var columnTitle = column.Title.Trim();
                    var rowIndex = rowIndex0 + 1;

                    return new Locator(
                        row: cell.Type switch
                        {
                            "GuiButton" => getButtonLabel((GuiButton)cell),
                            _ => rowIndex.ToString()
                        },
                        col: columnTitle
                    );
                }
            }
            catch (System.Exception) {}
        }
    }

    return null;
}

Locator? getLocator(GuiVComponent component)
{
    return component switch
    {
        GuiCheckBox checkbox => new Locator(getCheckBoxLabel(checkbox)),
        GuiComboBox comboBox => new Locator(getComboBoxLabel(comboBox)),
        GuiRadioButton radioButton => new Locator(getRadioButtonLabel(radioButton)),
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

string? getGridViewToolbarButtonLabel(GuiGridView gridView, string buttonId)
{
    for (int i = 0; i < gridView.ToolbarButtonCount; i++)
    {
        var id = gridView.GetToolbarButtonId(i);
        if (id == buttonId)
        {
            return gridView.GetToolbarButtonText(i).Trim().NullIfEmpty() ?? 
                   gridView.GetToolbarButtonTooltip(i).Trim();
        }
    }

    return null;
}

Locator getGridViewCellLocator(GuiGridView gridView, int rowIndex0, string columnId)
{
    var columnTitles = (GuiCollection)gridView.GetColumnTitles(columnId);
    var rowIndex = rowIndex0 + 1;

    if (columnTitles.Count > 0)
    {
        var columnTitle = (string)columnTitles.ElementAt(0);
        return new Locator(row: rowIndex.ToString(), col: columnTitle.Trim());
    }

    return new Locator(row: rowIndex.ToString(), col: gridView.GetColumnTooltip(columnId).Trim());
}

Event toEvent(object[] command, GuiComponent component)
{
    var type = command[0].ToString() switch {
        "M" => "Method",
        "SP" => "Property",
        _ => throw new Exception("Unknown command type")
    };
    var name = capitalize(command[1].ToString());
    var componentType = component.Type switch
    {
        "GuiDockShell" => "GuiContainerShell",
        "GuiShell" => "Gui" + ((GuiShell)component).SubType,
        _ => component.Type
    };
    var values = componentType switch
    {   "GuiComboBox" => [((GuiComboBox)component).Value],
        _ => command[2..].ToList()
    };
    var window = session.ActiveWindow.Text;
    var locator = componentType switch
    {
        "GuiGridView" => 
            (name, values) switch
            {
                ("PressToolbarButton", [string buttonId]) => new Locator(getGridViewToolbarButtonLabel((GuiGridView)component, buttonId)),
                ("ModifyCell", [int rowIndex0, string columnId, string value]) => getGridViewCellLocator((GuiGridView)component, rowIndex0, columnId),
                _ => null
            },
        "GuiTextField" when name == "SetFocus" =>
            component.Parent switch
            {
                GuiComponent parent when parent.Type == "GuiTableControl" => 
                    getTableCellLocator((GuiTableControl)parent, component.Id) with {row = ((GuiTextField)component).Text.Trim()},
                _ => new Locator(contents: ((GuiTextField)component).Text.Trim())
            },
        "GuiTree" =>
            (name, values) switch
            {
                ("DoubleClickItem" or "DoubleClickNode", [string nodeKey]) => new Locator(getTreeElementPath((GuiTree)component, nodeKey)),
                _ => null
            },
        _ => 
            component.Parent switch
            {
                GuiComponent parent when parent.Type == "GuiTableControl" => getTableCellLocator((GuiTableControl)parent, component.Id),
                _ => getLocator((GuiVComponent)component),
            }
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
    var objectTreeJson = session.GetObjectTree(
        componentId,
        new string[] { "Id", "Type", "Top", "Left", "Width", "Height", "Text", "Tooltip", "DefaultTooltip", "AccTooltip"}
    );
    var objectTree = JsonObject.Parse(objectTreeJson).AsObject();

    return JsonSerializer.Serialize(objectTree["children"][0]);
}


record KeyGuiEvent(string window, string action, string? role, Locator? locator, string? value)
{
    public override string ToString()
    {
        return $"Action: {action} | Role: {role} | Locator: {locator} | Value: {value}";
    }

    record KeywordCall(string name, Locator? locator, params string[] args)
    {
        public override string ToString()
        {
            return string.Join("    ", [name, locator, ..args.Select(arg => arg.NullIfEmpty() ?? "${EMPTY}")]);
        }
    }

    public string serialize(string lang)
    {
        var keywords = new {
            DoubleClickCell = new Dictionary<string, string> {
                ["DE"] = "Tabellenzelle doppelklicken",
                ["EN"] = "Double-click Cell"
            },
            DoubleClickTreeElement = new Dictionary<string, string> {
                ["DE"] = "Baumelement doppelklicken",
                ["EN"] = "Double-click Tree Element"
            },
            ExecuteTransaction = new Dictionary<string, string> {
                ["DE"] = "Transaktion ausführen",
                ["EN"] = "Execute Transaction"
            },
            FillCell = new Dictionary<string, string> {
                ["DE"] = "Tabellenzelle ausfüllen",
                ["EN"] = "Fill Cell"
            },
            FillTextField = new Dictionary<string, string> {
                ["DE"] = "Textfeld ausfüllen",
                ["EN"] = "Fill Text Field"
            },
            PressKey = new Dictionary<string, string> {
                ["DE"] = "Tastenkombination drücken",
                ["EN"] = "Press Key Combination"
            },
            PushButton = new Dictionary<string, string> {
                ["DE"] = "Knopf drücken",
                ["EN"] = "Push Button"
            },
            PushButtonCell = new Dictionary<string, string> {
                ["DE"] = "Tabellenzelle drücken",
                ["EN"] = "Push Button Cell"
            },
            SelectCell = new Dictionary<string, string> {
                ["DE"] = "Tabellenzelle markieren",
                ["EN"] = "Select Cell"
            },
            SelectCellValue = new Dictionary<string, string> {
                ["DE"] = "Tabellenzellenwert auswählen",
                ["EN"] = "Select Cell Value"
            },
            SelectComboBox = new Dictionary<string, string> {
                ["DE"] = "Auswahlmenüeintrag auswählen",
                ["EN"] = "Select Dropdown Menu Entry"
            },
            SelectRadio = new Dictionary<string, string> {
                ["DE"] = "Optionsfehld auswählen",
                ["EN"] = "Select Radio Button"
            },
            SelectTab = new Dictionary<string, string> {
                ["DE"] = "Reiter auswählen",
                ["EN"] = "Select Tab"
            },
            SelectTextField = new Dictionary<string, string> {
                ["DE"] = "Textfeld markieren",
                ["EN"] = "Select Text Field"
            },
            TickCheckbox = new Dictionary<string, string> {
                ["DE"] = "Formularfeld ankreuzen",
                ["EN"] = "Tick Checkbox"
            },
            TickCheckboxCell = new Dictionary<string, string> {
                ["DE"] = "Tabellenzelle ankreuzen",
                ["EN"] = "Tick Checkbox Cell"
            },
            UntickCheckbox = new Dictionary<string, string> {
                ["DE"] = "Formularfeld abwählen",
                ["EN"] = "Untick Checkbox"
            },
            UntickCheckboxCell = new Dictionary<string, string> {
                ["DE"] = "Tabellenzelle abwählen",
                ["EN"] = "Untick Checkbox Cell"
            }
        };

        var keywordCall = (action, role, value) switch
        {
            (KeyGuiActions.Check, KeyGuiRoles.Cell, _) => new KeywordCall(keywords.TickCheckboxCell[lang], locator),
            (KeyGuiActions.Check, KeyGuiRoles.Checkbox, _) => new KeywordCall(keywords.TickCheckbox[lang], locator),
            (KeyGuiActions.Click, KeyGuiRoles.Cell, _) => new KeywordCall(keywords.SelectCell[lang], locator),
            (KeyGuiActions.Click, KeyGuiRoles.Radio, _) => new KeywordCall(keywords.SelectRadio[lang], locator),
            (KeyGuiActions.Click, KeyGuiRoles.Tab, _) => new KeywordCall(keywords.SelectTab[lang], locator),
            (KeyGuiActions.Click, KeyGuiRoles.TextField, _) => new KeywordCall(keywords.SelectTextField[lang], locator),
            (KeyGuiActions.DoubleClick, KeyGuiRoles.TreeElement, _) => new KeywordCall(keywords.DoubleClickTreeElement[lang], locator),
            (KeyGuiActions.DoubleClick, KeyGuiRoles.Cell, _) => new KeywordCall(keywords.DoubleClickCell[lang], locator),
            (KeyGuiActions.Execute, _, string tCode) => new KeywordCall(keywords.ExecuteTransaction[lang], null, tCode),
            (KeyGuiActions.Fill, KeyGuiRoles.Cell, string contents) => new KeywordCall(keywords.FillCell[lang], locator, contents),
            (KeyGuiActions.Fill, KeyGuiRoles.TextField, string contents) => new KeywordCall(keywords.FillTextField[lang], locator, contents),
            (KeyGuiActions.PressKey, _, string key) => new KeywordCall(keywords.PressKey[lang], null, key),
            (KeyGuiActions.Push, KeyGuiRoles.Button, _) => new KeywordCall(keywords.PushButton[lang], locator),
            (KeyGuiActions.Push, KeyGuiRoles.Cell, _) => new KeywordCall(keywords.PushButtonCell[lang], locator),
            (KeyGuiActions.Select, KeyGuiRoles.Cell, string value) => new KeywordCall(keywords.SelectCellValue[lang], locator, value),
            (KeyGuiActions.Select, KeyGuiRoles.Combobox, string option) => new KeywordCall(keywords.SelectComboBox[lang], locator, option),
            (KeyGuiActions.Uncheck, KeyGuiRoles.Cell, _) => new KeywordCall(keywords.UntickCheckboxCell[lang], locator),
            (KeyGuiActions.Uncheck, KeyGuiRoles.Checkbox, _) => new KeywordCall(keywords.UntickCheckbox[lang], locator),
            _ => new KeywordCall("Fail", null, $"Unknown Keyword: {action} {role}")
        };

        return keywordCall.ToString();
    }
}
var keyGuiEventLog = new List<KeyGuiEvent>();

static class KeyGuiActions
{
    public const string Check = "check";
    public const string Click = "click";
    public const string DoubleClick = "double_click";
    public const string Execute = "execute";
    public const string Fill = "fill";
    public const string PressKey = "press_key";
    public const string Push = "push";
    public const string Select = "select";
    public const string SelectRow = "select_row";
    public const string Uncheck = "uncheck";
}

static class KeyGuiLocators
{
    public const string HLabel = "hlabel";
    public const string VLabel = "vlabel";
}

static class KeyGuiRoles
{
    public const string Button = "button";
    public const string Cell = "cell";
    public const string Checkbox = "checkbox";
    public const string Combobox = "combobox";
    public const string MultiLineTextField = "multiline_textfield";
    public const string Radio = "radio";
    public const string Tab = "tab";
    public const string TextField = "textfield";
    public const string TreeElement = "tree_element";
}

KeyGuiEvent? toKeyGuiEvent(List<Event> events)
{
    if (events.Count == 0) return null;

    var component = session.FindById(events[0].componentId);
    var componentType = events[0].componentType;
    var locator = events[0].locator;

    return componentType switch
    {
        "GuiButton" => events switch
        {
            [{window: string window, type: "Method", name: "Press"}] => new KeyGuiEvent(
                window,
                KeyGuiActions.Push,
                locator.col != null ? KeyGuiRoles.Cell : KeyGuiRoles.Button,
                locator,
                null
            ),
            _ => null
        },
        "GuiCheckBox" => events switch
        {
            [{window: string window, type: "Property", name: "Selected", values: [bool selected]}] => new KeyGuiEvent(
                window,
                selected ? KeyGuiActions.Check: KeyGuiActions.Uncheck,
                locator.col != null ? KeyGuiRoles.Cell : KeyGuiRoles.Checkbox,
                locator,
                null
            ),
            _ => null
        },
        "GuiComboBox" => events switch
        {
            [{window: string window, type: "Property", name: "Key", values: [string value]}] => new KeyGuiEvent(
                window,
                KeyGuiActions.Select,
                KeyGuiRoles.Combobox,
                locator,
                value
            ),
            _ => null
        },
        "GuiGridView" => events switch
        {
            [{window: string window, type: "Method", name: "PressToolbarButton", values: [string name]}] => new KeyGuiEvent(
                window,
                KeyGuiActions.Push,
                KeyGuiRoles.Button,
                locator,
                null
            ),
            [{window: string window, type: "Method", name: "ModifyCell", values: [int rowIndex, string colId, string value]}] => new KeyGuiEvent(
                window,
                ((GuiGridView)component).GetCellType(rowIndex, colId) switch
                {
                    "Normal" => KeyGuiActions.Fill,
                    "ValueList" => KeyGuiActions.Select,
                    _ => null
                },
                KeyGuiRoles.Cell,
                locator,
                value
            ),
            _ => null
        },
        "GuiMainWindow" or "GuiModalWindow" => events switch
        {
            [{name: "SendVKey"}] when keyGuiEventLog.Last().action == KeyGuiActions.Execute => null,
            [{window: string window, type: "Method", name: "SendVKey", values: [int vkey]}] => 
                new KeyGuiEvent(
                    window,
                    KeyGuiActions.PressKey,
                    null,
                    null,
                    vkey switch
                    {
                        0 => "Enter",
                        1 => "F1",
                        2 => "F2",
                        3 => "F3",
                        4 => "F4",
                        5 => "F5",
                        6 => "F6",
                        7 => "F7",
                        8 => "F8",
                        9 => "F9",
                        10  => "F10",
                        12 => "F12",
                        _ => vkey.ToString()
                    }
                ),
            _ => null
        },
        "GuiOkCodeField" => events switch
        {
            [{window: string window, type: "Property", name: "Text", values: [string t_code]}] =>
                new KeyGuiEvent(
                    window,
                    KeyGuiActions.Execute,
                    null,
                    null,
                    t_code
                ),
             _ => null
        },
        "GuiRadioButton" => events switch
        {
            [{window: string window, type: "Method", name: "Select"}] => new KeyGuiEvent(
                window,
                KeyGuiActions.Click,
                KeyGuiRoles.Radio,
                locator,
                null
            ),
            _ => null
        },
        "GuiTab" => events switch
        {
            [{window: string window, type: "Method", name: "Select"}] => new KeyGuiEvent(
                window,
                KeyGuiActions.Click,
                KeyGuiRoles.Tab,
                locator,
                null
            ),
             _ => null
        },
        "GuiTableControl" => events switch
        {
            [{window: string window, type: "Method", name: "GetAbsoluteRow", values: [int row]},
            {type: "Property", name: "Selected"}] => new KeyGuiEvent(
                window,
                KeyGuiActions.SelectRow,
                null,
                null,
                (row + 1).ToString()
            ),
             _ => null
        },
        "GuiTextEdit" => events switch
        {
            [{window: string window, type: "Property", name: "Text", values: [string text]}] => new KeyGuiEvent(
                window,
                KeyGuiActions.Fill,
                KeyGuiRoles.MultiLineTextField,
                locator,
                text
            ),
            _ => null
        },
        "GuiCTextField" or "GuiTextField" or "GuiPasswordField" => events.Select(
            e => e switch
            {
                {name: "SetFocus"} when keyGuiEventLog.Last().action == KeyGuiActions.Fill && (keyGuiEventLog.Last().locator == locator || keyGuiEventLog.Last().locator.contents == locator.row) => null,
                {type: "Method", name: "SetFocus"} => new KeyGuiEvent(
                    e.window,
                    KeyGuiActions.Click,
                    locator.col != null ? KeyGuiRoles.Cell : KeyGuiRoles.TextField,
                    locator,
                    null
                ),
                {type: "Property", name: "Text", values: [string value]} => new KeyGuiEvent(
                    e.window,
                    KeyGuiActions.Fill,
                    locator.col != null ? KeyGuiRoles.Cell : KeyGuiRoles.TextField,
                    locator,
                    value
                ),
                _ => null
            }
        ).Where(e => e != null).FirstOrDefault(),
        "GuiTree" => events switch
        {
            [{window: string window, type: "Method", name: "DoubleClickItem" or "DoubleClickNode"}] => new KeyGuiEvent(
                window,
                KeyGuiActions.DoubleClick,
                KeyGuiRoles.TreeElement,
                locator,
                null
            ),
             _ => null
        },
        _ => null
    };
}

void saveKeyGui(string filename)
{
    var json = JsonSerializer.Serialize(keyGuiEventLog);

    File.WriteAllText(
        Path.Combine(Directory.GetCurrentDirectory(), filename),
        JsonToYaml(json)    
    );
}

void saveRobotFile(string testcase, string lang)
{
    var library = lang switch
    {
        "DE" => "RoboSAPiens.DE",
        _ => "RoboSAPiens",
    };

    var test_setup = lang switch
    {
        "DE" => "Laufende SAP GUI übernehmen",
        _ => "Connect to running SAP",
    };

    var template = $"""
    *** Settings ***
    Library     {library}    x64=True
    Test Setup   {test_setup}

    *** Test Cases ***
    {testcase}

    """;

    File.WriteAllText(
        Path.Combine(Directory.GetCurrentDirectory(), testcase + ".robot"),
        template + string.Join(Environment.NewLine, keyGuiEventLog.Select(e => "    " + e.serialize(lang)))
    );
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

void saveObjectTree(string componentId, string filename)
{
    File.WriteAllText(
        Path.Combine(Directory.GetCurrentDirectory(), filename),
        JsonToYaml(getObjectTree(componentId))
    );
}
