using System.IO.Hashing;
using System.Reflection;
using System.Text;
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
    [JsonSerializable(typeof(Recording))]
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

    class VKeys
    {
        static string[] keyBindings =
        {
            "Enter",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "Ctrl+S",
            "F12",
            "Shift+F1",
            "Shift+F2",
            "Shift+F3",
            "Shift+F4",
            "Shift+F5",
            "Shift+F6",
            "Shift+F7",
            "Shift+F8",
            "Shift+F9",
            "Shift+Ctrl+0",
            "Shift+F11",
            "Shift+F12",
            "Ctrl+F1",
            "Ctrl+F2",
            "Ctrl+F3",
            "Ctrl+F4",
            "Ctrl+F5",
            "Ctrl+F6",
            "Ctrl+F7",
            "Ctrl+F8",
            "Ctrl+F9",
            "Ctrl+F10",
            "Ctrl+F11",
            "Ctrl+F12",
            "Ctrl+Shift+F1",
            "Ctrl+Shift+F2",
            "Ctrl+Shift+F3",
            "Ctrl+Shift+F4",
            "Ctrl+Shift+F5",
            "Ctrl+Shift+F6",
            "Ctrl+Shift+F7",
            "Ctrl+Shift+F8",
            "Ctrl+Shift+F9",
            "Ctrl+Shift+F10",
            "Ctrl+Shift+F11",
            "Ctrl+Shift+F12",
            "VKEY-49",
            "Ctrl+E",
            "Ctrl+F",
            "Ctrl+/",
            "Ctrl+\\",
            "Ctrl+N",
            "Ctrl+O",
            "Ctrl+X",
            "Ctrl+C",
            "Ctrl+V",
            "Ctrl+Z",
            "Ctrl+PageUp",
            "PageUp",
            "PageDown",
            "Ctrl+PageDown",
            "Ctrl+G",
            "Ctrl+R",
            "Ctrl+P"
        };

        public static string getKeyBinding(int vkey)
        {
            return vkey switch
            {
                int when vkey < 49 => keyBindings[vkey],
                int when vkey >= 70 => keyBindings[vkey-20],
                _ => throw new Exception("Invalid VKey.")
            };
        }
    }
    
    public record Recording(Dictionary<string, string> windows, List<KeyGuiEvent> keyGuiEvents);

    record Event(Window window, string componentId, string componentType, Locator? locator, string type, string name, List<object> values)
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

    public record Locator(string? hLabel=null, string? vLabel=null, string? contents=null, string? row=null, string? col=null)
    {
        string escapeSpaces(string s)
        {
            return 
                Regex.Matches(s, @"\s\s+")
                .Select(m => m.ToString())
                .Aggregate(s, (acc, m) => acc.Replace(m, " " + m[1..].Replace(" ", @"\ ")));
        }

        public override string ToString()
        {
            return (hLabel, vLabel, contents, row, col) switch
            {
                (string hLabel, null, null, null, null) => escapeSpaces(hLabel),
                (string hLabel, string vLabel, null, null, null) => $"{escapeSpaces(hLabel)} @ {escapeSpaces(vLabel)}",
                (null, string vLabel, null, null, null) => $"@ {escapeSpaces(vLabel)}",
                (null, null, string contents, null, null) => $"= {escapeSpaces(contents)}",
                (null, null, null, string row, string col) => $"{escapeSpaces(row)}    {escapeSpaces(col)}",
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
    
    public record KeywordCall(string name, Locator? locator, params string[] args)
    {
        public override string ToString()
        {
            return string.Join("    ", [name, locator, ..args.Select(arg => arg.NullIfEmpty() ?? "${EMPTY}")]);
        }
    }

    public record Window(string hash, string title);

    public record KeyGuiEvent(Window window, string action, string? role, Locator? locator, string? value)
    {
        public override string ToString()
        {
            return $"Action: {action} | Role: {role} | Locator: {locator} | Value: {value}";
        }

        // TODO: Define the method serializeKeyTA
        // - For each KeywordCall create an Action
        // - Group the Actions by Window. For each group create a Sequence
        // - Write the Test Case in terms of Sequence calls
        
        public string serialize(string lang)
        {
            return toKeywordCall(lang).ToString();
        }

        public KeywordCall toKeywordCall(string lang)
        {
            var exact = new Dictionary<string, string> {
                ["DE"] = "exakt",
                ["EN"] = "exact"
            };
            var keywords = new {
                ConnectToSap = new Dictionary<string, string> {
                    ["DE"] = "Laufende SAP GUI übernehmen",
                    ["EN"] = "Connect To Running SAP"
                },
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
                    ["DE"] = "Optionsfeld auswählen",
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
                (KeyGuiActions.Connect, _, null) => new KeywordCall(keywords.ConnectToSap[lang], null, []),
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
                (KeyGuiActions.Push, KeyGuiRoles.Button, _) => new KeywordCall(keywords.PushButton[lang], locator, [$"{exact[lang]}=True"]),
                (KeyGuiActions.Push, KeyGuiRoles.Cell, _) => new KeywordCall(keywords.PushButtonCell[lang], locator),
                (KeyGuiActions.Select, KeyGuiRoles.Cell, string value) => new KeywordCall(keywords.SelectCellValue[lang], locator, value),
                (KeyGuiActions.Select, KeyGuiRoles.Combobox, string option) => new KeywordCall(keywords.SelectComboBox[lang], locator, option),
                (KeyGuiActions.Uncheck, KeyGuiRoles.Cell, _) => new KeywordCall(keywords.UntickCheckboxCell[lang], locator),
                (KeyGuiActions.Uncheck, KeyGuiRoles.Checkbox, _) => new KeywordCall(keywords.UntickCheckbox[lang], locator),
                _ => new KeywordCall("Fail", null, $"Unknown Keyword: {action} {role}")
            };

            return keywordCall;
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
        Dictionary<string, string> windows = [];
        GuiSession session;

        public GuiRecorder(bool debug)
        {
            this.debug = debug;
            session = getSession();
        }

        string crc32(string text)
        {
            return BitConverter.ToString(Crc32.Hash(Encoding.UTF8.GetBytes(text)));
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
                var session = (GuiSession)connection.Sessions.ElementAt(0);
                var windowTitle = session.ActiveWindow.Text;
                var windowHash = crc32(windowTitle);
                var connectionDescription = session.Info.Client switch
                {
                    "000" => connection.Description,
                    _ => null
                };

                keyGuiEventLog.Add(new KeyGuiEvent(new Window(windowHash, windowTitle), KeyGuiActions.Connect, null, null, connectionDescription));

                return session;
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
            var label = 
                button.Text.Trim().NullIfEmpty() ?? 
                button.Tooltip.Trim();

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
            var verticalAlignedLabels =
                parentObject.children
                .Select(obj => obj.properties)
                .Where(obj =>
                    (obj.Type == "GuiLabel" || (obj.Type == "GuiTextField" && obj.Changeable == "false")) &&
                    Math.Abs(obj.Top - component.Top) < 5
                );
            var closestLeftLabel = 
                verticalAlignedLabels
                .Where(label =>
                    label.Left < component.Left && 
                    Math.Abs(label.Left + label.Width - component.Left) < 30
                )
                .MinBy(label => Math.Abs(label.Left + label.Width - component.Left))
                ?.Text.Trim();
            var closestRightLabel = 
                verticalAlignedLabels
                .Where(label =>
                    label.Left > component.Left + component.Width && 
                    Math.Abs(label.Left - (component.Left + component.Width)) < 30
                )
                .MinBy(label => Math.Abs(label.Left - (component.Left + component.Width)))
                ?.Text.Trim();

            return closestLeftLabel ?? closestRightLabel ?? getTooltip(component);
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
                GuiRadioButton radioButton => new Locator(radioButton.Text.Trim().NullIfEmpty() ?? getLabel(component)),
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
            var columnTitle = gridView.GetDisplayedColumnTitle(columnId).Trim();
            var rowIndex = rowIndex0 + 1;

            if (columnTitle != "")
            {
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
            var windowTitle = session.ActiveWindow.Text;
            var windowHash = crc32(windowTitle);
            var screenshot = Convert.ToBase64String((byte[])session.ActiveWindow.HardCopyToMemory(1));
            windows.TryAdd(windowHash, screenshot);
            var locator = componentType switch
            {
                "GuiDialogShell" => null,
                "GuiGridView" => 
                    (name, values) switch
                    {
                        ("DoubleClickCurrentCell", _) => getGridViewCellLocator((GuiGridView)component, ((GuiGridView)component).CurrentCellRow, ((GuiGridView)component).CurrentCellColumn),
                        ("ModifyCell", [int rowIndex0, string columnId, _]) => getGridViewCellLocator((GuiGridView)component, rowIndex0, columnId),
                        ("PressToolbarButton", [string buttonId]) => new Locator(getGridViewToolbarButtonLabel((GuiGridView)component, buttonId)),
                        ("SetCurrentCell", [int rowIndex0, string columnId]) => getGridViewCellLocator((GuiGridView)component, rowIndex0, columnId),
                        _ => null
                    },
                "GuiMainWindow" => null,
                "GuiModalWindow" => null,
                "GuiTextField" when name == "SetFocus" =>
                    component.Parent switch
                    {
                        GuiComponent parent when parent.Type == "GuiTableControl" =>
                            ((GuiTableControl)parent).Columns switch
                            {
                                GuiCollection columns when columns.Count == 1 && ((GuiTableColumn)columns.ElementAt(0)).Title == "" => new Locator(contents: ((GuiTextField)component).Text.Trim()),
                                _ => getTableCellLocator((GuiTableControl)parent, component.Id) with {row = ((GuiTextField)component).Text.Trim()},
                            },
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

            return new Event(new Window(windowHash, windowTitle), component.Id, componentType, locator, type, name, values);
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

        SapObject getSapObject(string componentId)
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            return (JsonSerializer.Deserialize(getObjectTree(componentId), typeof(SapObject), new SerializerContext(options)) as SapObject)!;
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
            var lastKeyGuiEvent = keyGuiEventLog.Last();

            return componentType switch
            {
                "GuiButton" => events switch
                {
                    [{window: Window window, type: "Method", name: "Press"}] => new KeyGuiEvent(
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
                    [{window: Window window, type: "Set Property", name: "Selected", values: [bool selected]}] => new KeyGuiEvent(
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
                    [{window: Window window, type: "Set Property", name: "Key", values: [string value]}] => new KeyGuiEvent(
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
                    [{window: Window window, type: "Method", name: "PressToolbarButton", values: [string name]}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.Push,
                        KeyGuiRoles.Button,
                        locator,
                        null
                    ),
                    [{window: Window window, type: "Method", name: "ModifyCell", values: [int rowIndex, string colId, string value]}] => new KeyGuiEvent(
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
                    [{ window: Window window, type: "Method", name: "DoubleClickCurrentCell" }] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.DoubleClick,
                        KeyGuiRoles.Cell,
                        locator,
                        null
                    ),
                    [{ window: Window window, type: "Method", name: "SetCurrentCell", values: [int rowIndex, string colId] }] => new KeyGuiEvent(
                        window,
                        ((GuiGridView)component).GetCellType(rowIndex, colId) switch
                        {
                            "Normal" => KeyGuiActions.Select,
                            "ValueList" => KeyGuiActions.Select,
                            _ => throw new Exception("Invalid cell type")
                        },
                        KeyGuiRoles.Cell,
                        locator,
                        null
                    ),
                    _ => null
                },
                "GuiLabel" => events switch
                {
                    [{window: Window window, name:"SetFocus"}] => new KeyGuiEvent(
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
                    [{window: Window window, type: "Method", name: "SendVKey", values: [int vkey]}] => 
                        new KeyGuiEvent(
                            window,
                            KeyGuiActions.PressKey,
                            null,
                            null,
                            VKeys.getKeyBinding(vkey)
                        ),
                    _ => null
                },
                "GuiOkCodeField" => events switch
                {
                    [{window: Window window, type: "Set Property", name: "Text", values: [string t_code]}] =>
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
                    [{window: Window window, type: "Method", name: "Select"}] => new KeyGuiEvent(
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
                    [{window: Window window, type: "Method", name: "Select"}] => new KeyGuiEvent(
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
                    [{window: Window window, type: "Method", name: "GetAbsoluteRow", values: [int row]},
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
                    [{window: Window window, type: "Set Property", name: "Text", values: [string text]}] => new KeyGuiEvent(
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
                        {name: "SetFocus"} when lastKeyGuiEvent.action == KeyGuiActions.Fill && eventLog.Last().componentId == component.Id => null,
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
                    [{window: Window window, type: "Method", name: "DoubleClickItem" or "DoubleClickNode"}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.DoubleClick,
                        KeyGuiRoles.TreeElement,
                        locator,
                        null
                    ),
                    [{window: Window window, type: "Method", name: "PressButton"}] => new KeyGuiEvent(
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

        void saveAsJson(object? value, Type type, string filename)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(value, type, new SerializerContext(options));

            File.WriteAllText(
                Path.Combine(Directory.GetCurrentDirectory(), filename + ".json"),
                json
            );
        }

        public void saveKeyGui(string filename)
        {
            saveAsJson(new Recording(windows, keyGuiEventLog), typeof(Recording), filename);
        }

        public static string toRobotFile(List<KeyGuiEvent> keyGuiEventLog, string testcase, string lang)
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

            return template + string.Join(Environment.NewLine, keyGuiEventLog.Select(e => "    " + e.serialize(lang)));
        }

        public void saveRobotFile(string testcase, string lang)
        {
            File.WriteAllText(
                Path.Combine(Directory.GetCurrentDirectory(), testcase + ".robot"),
                toRobotFile(keyGuiEventLog, testcase, lang)
            );
        }

        public void saveEventLog(string filename)
        {
            saveAsJson(eventLog, typeof(List<Event>), filename);
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
