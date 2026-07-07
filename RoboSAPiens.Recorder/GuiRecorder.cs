using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using sapfewse;
using saprotwr.net;

namespace RoboSAPiens.Recorder
{
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(JsonObject))]
    [JsonSerializable(typeof(List<Event>))]
    [JsonSerializable(typeof(List<KeyGuiEvent>))]
    [JsonSerializable(typeof(SapObject))]
    internal partial class SerializerContext : JsonSerializerContext {}

    public class ObjectToInferredTypesConverter: JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) => reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Number when reader.TryGetInt32(out int l) => l,
                JsonTokenType.String => reader.GetString()!,
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
            };

        public override void Write(Utf8JsonWriter writer, object objectToWrite, JsonSerializerOptions options) {}  
    }
    
    record Event(string window, string componentId, string componentType, Locator? locator, string type, string name, List<object> values)
    {
        public string serialize()
        {
            var formatValue = (object val) => val switch
            {
                bool b => val.ToString()!.ToLower(),
                string s => $"\"{s}\"",
                _ => val.ToString()
            };
            var target = $"""(({componentType})session.FindById("{componentId}")).{name}""";

            return type switch
            {
                "Method" => target + "(" + string.Join(", ", values.Select(formatValue)) + ")",
                "Set Property" => target + " = " + formatValue(values[0]),
                _ => throw new Exception("Unknown event type")
            };
        }

        public override string ToString()
        {
            return $"Type: {type} | Name: {name} | Values: {string.Join(", ", values)}";
        }
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

    static class KeyGuiActions
    {
        public const string Check = "check";
        public const string Click = "click";
        public const string Connect = "connect";
        public const string DoubleClick = "double_click";
        public const string Execute = "execute";
        public const string Fill = "fill";
        public const string PressKey = "press_key";
        public const string Push = "push";
        public const string Select = "select";
        public const string SelectRow = "select_row";
        public const string Uncheck = "uncheck";
    }

    static class KeyGuiRoles
    {
        public const string Button = "button";
        public const string Cell = "cell";
        public const string Checkbox = "checkbox";
        public const string Combobox = "combobox";
        public const string Label = "label";
        public const string MultiLineTextField = "multiline_textfield";
        public const string Radio = "radio";
        public const string Tab = "tab";
        public const string TextField = "textfield";
        public const string TreeElement = "tree_element";
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

        // TODO: Define the method serializeKeyTA
        // - For each KeywordCall create an Action
        // - Group the Actions by Window. For each group create a Sequence
        // - Write the Test Case in terms of Sequence calls
        
        public string serialize(string lang)
        {
            var keywords = new {
                ConnectToServer = new Dictionary<string, string> {
                    ["DE"] = "Verbindung zum Server herstellen",
                    ["EN"] = "Connect To Server"
                },
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
                SelectText = new Dictionary<string, string> {
                    ["DE"] = "Text markieren",
                    ["EN"] = "Select Text"
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
                (KeyGuiActions.Connect, _, string connection) => new KeywordCall(keywords.ConnectToServer[lang], null, connection),
                (KeyGuiActions.Check, KeyGuiRoles.Cell, _) => new KeywordCall(keywords.TickCheckboxCell[lang], locator),
                (KeyGuiActions.Check, KeyGuiRoles.Checkbox, _) => new KeywordCall(keywords.TickCheckbox[lang], locator),
                (KeyGuiActions.Click, KeyGuiRoles.Cell, _) => new KeywordCall(keywords.SelectCell[lang], locator),
                (KeyGuiActions.Click, KeyGuiRoles.Label, _) => new KeywordCall(keywords.SelectText[lang], locator),
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
    
    record SapProperties(
        string Id, 
        string Type, 
        string Changeable,
        int Top, 
        int Left, 
        int Width, 
        int Height, 
        string Text, 
        string Tooltip, 
        string DefaultTooltip, 
        string AccTooltip
    );

    record SapObject(SapProperties properties, List<SapObject> children);
    
    static class Extensions
    {
        public static string? NullIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }
    }

    public class NoSapException : Exception
    {
        public NoSapException(string message) : base(message) {}
    }

    public class GuiRecorder
    {
        bool debug;
        List<Event> eventLog = [];
        List<KeyGuiEvent> keyGuiEventLog = [];
        GuiSession session;

        public GuiRecorder(bool debug)
        {
            this.debug = debug;
            session = getSession();
        }

        GuiSession getSession()
        {
            var rot = new CSapROTWrapper();
            var sapGui = rot.GetROTEntry("SAPGUI") ?? throw new NoSapException("SAP Logon is not running.");
            var sap = (GuiApplication)sapGui.GetType().InvokeMember(
                "GetScriptingEngine",
                BindingFlags.InvokeMethod,
                null,
                sapGui,
                null
            )!;
            try
            {
                var connection = (GuiConnection)sap.Connections.ElementAt(0);
                keyGuiEventLog.Add(new KeyGuiEvent("SAP Logon", KeyGuiActions.Connect, null, null, connection.Description));
                return (GuiSession)connection.Sessions.ElementAt(0); 
            }
            catch (Exception)
            {
                throw new NoSapException("Not connected to any SAP system.");
            }
        }

        static string capitalize(string s)
        {
            return char.ToUpper(s[0]) + s[1..];
        }

        void handleChange(GuiSession session, GuiComponent component, object commmandArray)
        {
            if (debug) Console.WriteLine("============");
            if (debug) Console.WriteLine($"Id: {component.Id}");
            if (debug) Console.WriteLine($"Type: {component.Type}");
            
            try
            {
                var commands = (object[])commmandArray;
                var events = commands.Select(command => toEvent((object[])command, component)).ToList();
                eventLog.AddRange(events);
                if (debug) Console.WriteLine("~~ Events ~~");
                if (debug) events.ForEach(Console.WriteLine);

                var keyGuiEvent = toKeyGuiEvent(events);
                if (keyGuiEvent != null) keyGuiEventLog.Add(keyGuiEvent);
            }
            catch (Exception ex)
            {
                if (debug) Console.WriteLine(ex);
            }

            if (debug) Console.WriteLine("============");
        }

        string getTooltip(GuiVComponent component)
        {
            return component.AccTooltip.Trim().NullIfEmpty() ??
                component.DefaultTooltip.Trim().NullIfEmpty() ??
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

        string getTabLabel(GuiTab tab)
        {
            return tab.Text.Trim().NullIfEmpty() ??
                getTooltip((GuiVComponent)tab);
        }

        string getLabel(GuiVComponent component)
        {
            var parentObject = getSapObject(component.Parent.Id);
            var closestLabel =
                parentObject?.children
                .Select(e => e.properties)
                .Where(e =>
                    (e.Type == "GuiLabel" || (e.Type == "GuiTextField" && e.Changeable == "false")) &&
                    e.Left < component.Left &&
                    Math.Abs(e.Top - component.Top) < 5 &&
                    Math.Abs(e.Left + e.Width - component.Left) < 30
                )
                .MinBy(e => Math.Abs(e.Left - component.Left));

            return closestLabel?.Text.Trim() ?? getTooltip(component);
        }

        Locator? getTableCellLocator(GuiTableControl table, string componentId)
        {
            for (int rowIndex0 = 0; rowIndex0 <= table.RowCount; rowIndex0++)
            {
                List<string> texts = [];
                for (int colIdx = 0; colIdx < table.Columns.Length; colIdx++) 
                {
                    try
                    {
                        var cell = table.GetCell(rowIndex0, colIdx);
                        if (!cell.Changeable && cell.Text != "")
                        {
                            texts.Add(cell.Text.Trim());
                        }
                        if (cell.Id == componentId)
                        {
                            var column = (GuiTableColumn)table.Columns.ElementAt(colIdx);
                            var columnTitle = column.Title.Trim();
                            var rowIndex = rowIndex0 + 1;

                            return new Locator(
                                row: cell.Type switch
                                {
                                    "GuiButton" => getButtonLabel((GuiButton)cell),
                                    _ => texts.MaxBy(t => t.Length) ?? rowIndex.ToString()
                                },
                                col: columnTitle
                            );
                        }
                    }
                    catch (Exception) {}
                }
            }

            return null;
        }

        Locator? getLocator(GuiVComponent component)
        {
            return component switch
            {
                GuiButton button => new Locator(getButtonLabel(button)),
                GuiCheckBox checkBox => new Locator(checkBox.Text.Trim().NullIfEmpty() ?? getLabel(component)),
                GuiLabel label => new Locator(contents: label.Text.Trim()),
                GuiOkCodeField => null,
                GuiTab tab => new Locator(getTabLabel(tab)),
                GuiTextField textField when !textField.Changeable => new Locator(contents: textField.Text.Trim()),
                _ => new Locator(getLabel(component))
            };
        }

        enum TreeType 
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

        Locator getTreeCellLocator(GuiTree tree, string nodeKey, string columnName)
        {
            var columnTitle = tree.GetColumnTitleFromName(columnName).Trim();
            var itemText = tree.GetItemText(nodeKey, columnName);
            var itemTooltip = tree.GetItemToolTip(nodeKey, columnName);

            return new Locator(
                row: itemText.NullIfEmpty() ?? itemTooltip,
                col: columnTitle
            );
        }

        Event toEvent(object[] command, GuiComponent component)
        {
            var type = command[0].ToString() switch {
                "M" => "Method",
                "GP" => "Get Property",
                "SP" => "Set Property",
                _ => throw new Exception("Unknown command type")
            };
            var name = capitalize(command[1].ToString()!);
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
                "GuiDialogShell" => null,
                "GuiGridView" => 
                    (name, values) switch
                    {
                        ("PressToolbarButton", [string buttonId]) => new Locator(getGridViewToolbarButtonLabel((GuiGridView)component, buttonId)),
                        ("ModifyCell", [int rowIndex0, string columnId, _]) => getGridViewCellLocator((GuiGridView)component, rowIndex0, columnId),
                        _ => null
                    },
                "GuiMainWindow" => null,
                "GuiModalWindow" => null,
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
                        ("PressButton", [string nodeKey, string column]) => getTreeCellLocator((GuiTree)component, nodeKey, column),
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

        public void recordStart()
        {
            session.Change += handleChange;
            session.Destroy += handleDestroy;
            session.Record = true;
        }

        public void recordStop()
        {
            session.Record = false;
            session.Change -= handleChange;
            session.Destroy -= handleDestroy;
        }

        void refresh()
        {
            session = getSession();
        }

        SapObject? getSapObject(string componentId)
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            return JsonSerializer.Deserialize(getObjectTree(componentId), typeof(SapObject), new SerializerContext(options)) as SapObject;
        }

        string getObjectTree(string componentId)
        {
            var objectTreeJson = session.GetObjectTree(
                componentId,
                typeof(SapProperties).GetProperties().Select(p => p.Name).ToArray()
            );
            var objectTree = JsonNode.Parse(objectTreeJson)!.AsObject();

            return JsonSerializer.Serialize(objectTree["children"]![0], typeof(JsonObject), new SerializerContext());
        }

        static class KeyGuiLocators
        {
            public const string HLabel = "hlabel";
            public const string VLabel = "vlabel";
        }

        KeyGuiEvent? toKeyGuiEvent(List<Event> events)
        {
            if (events.Count == 0) return null;

            var component = (GuiVComponent)session.FindById(events[0].componentId);
            var componentType = events[0].componentType;
            var locator = events[0].locator;

            return componentType switch
            {
                "GuiButton" => events switch
                {
                    [{window: string window, type: "Method", name: "Press"}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.Push,
                        locator?.col != null ? KeyGuiRoles.Cell : KeyGuiRoles.Button,
                        locator,
                        null
                    ),
                    _ => null
                },
                "GuiCheckBox" => events switch
                {
                    [{window: string window, type: "Set Property", name: "Selected", values: [bool selected]}] => new KeyGuiEvent(
                        window,
                        selected ? KeyGuiActions.Check: KeyGuiActions.Uncheck,
                        locator?.col != null ? KeyGuiRoles.Cell : KeyGuiRoles.Checkbox,
                        locator,
                        null
                    ),
                    _ => null
                },
                "GuiComboBox" => events switch
                {
                    [{window: string window, type: "Set Property", name: "Key", values: [string value]}] => new KeyGuiEvent(
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
                            _ => throw new Exception("Invalid cell type")
                        },
                        KeyGuiRoles.Cell,
                        locator,
                        value
                    ),
                    _ => null
                },
                "GuiLabel" => events switch
                {
                    [{window: string window, name:"SetFocus"}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.Click,
                        KeyGuiRoles.Label,
                        locator,
                        null
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
                    [{window: string window, type: "Set Property", name: "Text", values: [string t_code]}] =>
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
                    {type: "Set Property", name: "Selected"}] => new KeyGuiEvent(
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
                    [{window: string window, type: "Set Property", name: "Text", values: [string text]}] => new KeyGuiEvent(
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
                        {name: "SetFocus"} when eventLog.Last().componentId == component.Id => null,
                        {name: "SetFocus"} => new KeyGuiEvent(
                            e.window,
                            KeyGuiActions.Click,
                            locator?.col != null ? KeyGuiRoles.Cell : KeyGuiRoles.TextField,
                            locator,
                            null
                        ),
                        {type: "Set Property", name: "CaretPosition"} when !component.Changeable && keyGuiEventLog.Last().action != KeyGuiActions.Click => new KeyGuiEvent(
                            e.window,
                            KeyGuiActions.Click,
                            locator?.col != null ? KeyGuiRoles.Cell : KeyGuiRoles.TextField,
                            locator,
                            null
                        ),
                        {type: "Set Property", name: "Text", values: [string value]} => new KeyGuiEvent(
                            e.window,
                            KeyGuiActions.Fill,
                            locator?.col != null ? KeyGuiRoles.Cell : KeyGuiRoles.TextField,
                            locator,
                            value
                        ),
                        _ => null
                    }
                ).FirstOrDefault(e => e != null),
                "GuiTree" => events switch
                {
                    [{window: string window, type: "Method", name: "DoubleClickItem" or "DoubleClickNode"}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.DoubleClick,
                        KeyGuiRoles.TreeElement,
                        locator,
                        null
                    ),
                    [{window: string window, type: "Method", name: "PressButton"}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.Push,
                        KeyGuiRoles.Cell,
                        locator,
                        null
                    ),
                    _ => null
                },
                _ => null
            };
        }

        public void saveKeyGui(string filename)
        {
            var json = JsonSerializer.Serialize(keyGuiEventLog, typeof(List<KeyGuiEvent>), new SerializerContext());

            File.WriteAllText(
                Path.Combine(Directory.GetCurrentDirectory(), filename + ".json"),
                json
            );
        }

        public void saveRobotFile(string testcase, string lang)
        {
            var library = lang switch
            {
                "DE" => "RoboSAPiens.DE",
                _ => "RoboSAPiens",
            };

            var open_sap = lang switch
            {
                "DE" => "SAP starten",
                _ => "Open SAP",
            };

            var template = $"""
            *** Settings ***
            Library     {library}    x64=True
            Test Setup   {open_sap}    C:\\Program Files\\SAP\\FrontEnd\\SAPGUI\\saplogon.exe

            *** Test Cases ***
            {testcase}

            """;

            File.WriteAllText(
                Path.Combine(Directory.GetCurrentDirectory(), testcase + ".robot"),
                template + string.Join(Environment.NewLine, keyGuiEventLog.Select(e => "    " + e.serialize(lang)))
            );
        }

        public void saveEventLog(string filename)
        {
            var json = JsonSerializer.Serialize(eventLog, typeof(List<Event>), new SerializerContext());

            File.WriteAllText(
                Path.Combine(Directory.GetCurrentDirectory(), filename + ".json"),
                json
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
                getObjectTree(componentId)
            );
        }
    }
}
