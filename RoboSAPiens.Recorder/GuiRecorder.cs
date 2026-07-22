using System.Diagnostics;
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
    [JsonSerializable(typeof(List<KeywordCall>))]
    [JsonSerializable(typeof(KeywordRecording))]
    [JsonSerializable(typeof(SapObject))]
    internal partial class SerializerContext : JsonSerializerContext {}

    public record KeywordRecording(List<KeywordCall> keywordCalls, Dictionary<long, Window> windows);

    public record Window(long id, string title, byte[] screenshot)
    {
        public void saveScreenshot(string directory)
        {
            File.WriteAllBytes(
                Path.Combine(directory, id + ".png"),
                screenshot
            );
        }
    }

    public record Event(long window, string componentId, string componentType, Locator? locator, string type, string name, List<object> values)
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

    public record Locator(string? hLabel=null, string? vLabel=null, string? contents=null, string? row=null, string? col=null, int gridIndex=0)
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
            return (hLabel, vLabel, contents, row, col, gridIndex) switch
            {
                (string hLabel, null, null, null, null, 0) => escapeSpaces(hLabel),
                (string hLabel, string vLabel, null, null, null, 0) => $"{escapeSpaces(hLabel)} @ {escapeSpaces(vLabel)}",
                (string hLabel, string vLabel, null, null, null, 1) => $"{escapeSpaces(hLabel)} @@ {escapeSpaces(vLabel)}",
                (null, string vLabel, null, null, null, 0) => $"@ {escapeSpaces(vLabel)}",
                (null, null, string contents, null, null, 0) => $"= {escapeSpaces(contents)}",
                (null, null, null, string row, string col, 0) => $"{escapeSpaces(row)}    {escapeSpaces(col)}",
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
        public const string Expand = "expand";
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
    
    public record KeywordCall(long window, string name, Locator? locator, params string[] args)
    {
        public override string ToString()
        {
            return string.Join("    ", [name, locator, ..args.Select(arg => arg.NullIfEmpty() ?? "${EMPTY}")]);
        }
    }

    public record KeyGuiEvent(long window, string action, string? role, Locator? locator, string? value)
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
                DoubleClickTextField = new Dictionary<string, string> {
                    ["DE"] = "Textfeld doppelklicken",
                    ["EN"] = "Double-click Text Field"
                },
                DoubleClickTreeElement = new Dictionary<string, string> {
                    ["DE"] = "Baumelement doppelklicken",
                    ["EN"] = "Double-click Tree Element"
                },
                ExecuteTransaction = new Dictionary<string, string> {
                    ["DE"] = "Transaktion ausführen",
                    ["EN"] = "Execute Transaction"
                },
                ExpandTreeFolder = new Dictionary<string, string> {
                    ["DE"] = "Baumordner aufklappen",
                    ["EN"] = "Expand Tree Folder"
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
                (KeyGuiActions.Connect, _, string connection) => new KeywordCall(window, keywords.ConnectToServer[lang], null, connection),
                (KeyGuiActions.Connect, _, null) => new KeywordCall(window, keywords.ConnectToSap[lang], null, []),
                (KeyGuiActions.Check, KeyGuiRoles.Cell, _) => new KeywordCall(window, keywords.TickCheckboxCell[lang], locator),
                (KeyGuiActions.Check, KeyGuiRoles.Checkbox, _) => new KeywordCall(window, keywords.TickCheckbox[lang], locator),
                (KeyGuiActions.Click, KeyGuiRoles.Cell, _) => new KeywordCall(window, keywords.SelectCell[lang], locator),
                (KeyGuiActions.Click, KeyGuiRoles.Label, _) => new KeywordCall(window, keywords.SelectText[lang], locator),
                (KeyGuiActions.Click, KeyGuiRoles.Radio, _) => new KeywordCall(window, keywords.SelectRadio[lang], locator),
                (KeyGuiActions.Click, KeyGuiRoles.Tab, _) => new KeywordCall(window, keywords.SelectTab[lang], locator),
                (KeyGuiActions.Click, KeyGuiRoles.TextField, _) => new KeywordCall(window, keywords.SelectTextField[lang], locator),
                (KeyGuiActions.DoubleClick, KeyGuiRoles.Cell, _) => new KeywordCall(window, keywords.DoubleClickCell[lang], locator),
                (KeyGuiActions.DoubleClick, KeyGuiRoles.TextField, _) => new KeywordCall(window, keywords.DoubleClickTextField[lang], locator),
                (KeyGuiActions.DoubleClick, KeyGuiRoles.TreeElement, _) => new KeywordCall(window, keywords.DoubleClickTreeElement[lang], locator),
                (KeyGuiActions.Execute, _, string tCode) => new KeywordCall(window, keywords.ExecuteTransaction[lang], null, tCode),
                (KeyGuiActions.Expand, KeyGuiRoles.TreeElement, _) => new KeywordCall(window, keywords.ExpandTreeFolder[lang], locator),
                (KeyGuiActions.Fill, KeyGuiRoles.Cell, string contents) => new KeywordCall(window, keywords.FillCell[lang], locator, contents),
                (KeyGuiActions.Fill, KeyGuiRoles.TextField, string contents) => new KeywordCall(window, keywords.FillTextField[lang], locator, contents),
                (KeyGuiActions.PressKey, _, string key) => new KeywordCall(window, keywords.PressKey[lang], null, key),
                (KeyGuiActions.Push, KeyGuiRoles.Button, _) => new KeywordCall(window, keywords.PushButton[lang], locator, [$"{exact[lang]}=True"]),
                (KeyGuiActions.Push, KeyGuiRoles.Cell, _) => new KeywordCall(window, keywords.PushButtonCell[lang], locator),
                (KeyGuiActions.Select, KeyGuiRoles.Cell, string value) => new KeywordCall(window, keywords.SelectCellValue[lang], locator, value),
                (KeyGuiActions.Select, KeyGuiRoles.Combobox, string option) => new KeywordCall(window, keywords.SelectComboBox[lang], locator, option),
                (KeyGuiActions.Uncheck, KeyGuiRoles.Cell, _) => new KeywordCall(window, keywords.UntickCheckboxCell[lang], locator),
                (KeyGuiActions.Uncheck, KeyGuiRoles.Checkbox, _) => new KeywordCall(window, keywords.UntickCheckbox[lang], locator),
                _ => new KeywordCall(window, "Fail", null, $"Unknown Keyword: {action} {role}")
            };

            return keywordCall;
        }
    }
    
    record SapProperties(
        string Id, 
        string Type, 
        string Changeable,
        int ScreenTop, 
        int ScreenLeft, 
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
        Dictionary<string, Dictionary<string, Locator>> adhocGrids = [];
        bool debug;
        List<Event> eventLog = [];
        List<KeyGuiEvent> keyGuiEventLog = [];
        GuiSession session;
        List<Window> windows = [];

        public GuiRecorder(bool debug)
        {
            this.debug = debug;
            session = getSession();
        }

        byte[] getScreenshot(GuiFrameWindow window, string? id)
        {
            GuiVComponent? component = null;
            byte[] screenshot;

            if (id != null)
            {
                component = (GuiVComponent)session.FindById(id);
                component.Visualize(true);
            }
            
            screenshot = ScreenCapture.saveWindowImage(window.Handle, screenshot: true);
            
            if (component != null)
            {
                component.Visualize(false);
            }
            
            return screenshot;
        }

        long getTimestamp()
        {
            return Stopwatch.GetTimestamp();
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
                var connectionDescription = session.Info.Client switch
                {
                    "000" => connection.Description,
                    _ => null
                };
                var windowId = getTimestamp();
                var window = session.ActiveWindow;

                keyGuiEventLog.Add(new KeyGuiEvent(windowId, KeyGuiActions.Connect, null, null, connectionDescription));
                windows.Add(new Window(windowId, window.Text, getScreenshot(window, id: null)));

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

                var last2 = keyGuiEventLog.TakeLast(2).ToList() switch
                {
                    [{action: KeyGuiActions.Click, role: KeyGuiRoles.TextField or KeyGuiRoles.Cell} e, 
                     {action: KeyGuiActions.PressKey, value: "F2"}] 
                     => [e with {action = KeyGuiActions.DoubleClick}],
                    _ => new List<KeyGuiEvent>()
                };

                if (last2.Count > 0)
                {
                    keyGuiEventLog = [..keyGuiEventLog.SkipLast(2), ..last2]; 
                }
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

            return Regex.Replace(label, @"\s\s+", " ");
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
                    Math.Abs(obj.ScreenTop - component.ScreenTop) < 5
                );
            var closestLeftLabel = 
                verticalAlignedLabels
                .Where(label =>
                    label.ScreenLeft < component.Left && 
                    Math.Abs(label.ScreenLeft + label.Width - component.Left) < 30
                )
                .MinBy(label => Math.Abs(label.ScreenLeft + label.Width - component.Left))
                ?.Text.Trim();
            var closestRightLabel = 
                verticalAlignedLabels
                .Where(label =>
                    label.ScreenLeft > component.Left + component.Width && 
                    Math.Abs(label.ScreenLeft - (component.Left + component.Width)) < 30
                )
                .MinBy(label => Math.Abs(label.ScreenLeft - (component.Left + component.Width)))
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
            if (Regex.IsMatch(component.Id, @"\[\d+,\d+\]$", RegexOptions.Compiled))
            {
                var adhocGridId = component.Parent.Id;

                if (!adhocGrids.ContainsKey(adhocGridId))
                {
                    var parentObject = getSapObject(adhocGridId);
                    var grid = 
                        parentObject.children
                        // Each group might be divided into two sets with different widths,
                        // corresponding to the primary grid and the secondary grid.
                        .GroupBy(obj => new { obj.properties.ScreenLeft })
                        .ToDictionary(
                            g => g.Key.ScreenLeft, 
                            g => g.ToList()
                                  .GroupBy(obj => new {obj.properties.Width})
                                  .Select(g => g.OrderBy(obj => obj.properties.ScreenTop).ToList())
                                  .ToList()
                        );
                    var columns = grid.Keys.ToHashSet();
                    var grandParentObject = getSapObject(((GuiVComponent)component.Parent).Parent.Id);
                    var guiBox = grandParentObject.children.Find(obj => obj.properties.Type == "GuiBox");
                    var firstElement = parentObject.children.First();
                    var columnTitles = 
                        grandParentObject.children
                        .Where(obj => obj.properties.Type == "GuiLabel")
                        .Where(label => label.properties.ScreenTop > guiBox!.properties.ScreenTop)
                        .Where(label => label.properties.ScreenTop < firstElement.properties.ScreenTop)
                        .Select(label => new {label, col=columns.MinBy(col => Math.Abs(col - label.properties.ScreenLeft))})
                        .GroupBy(_ => _.col)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(_ => _.label.properties.Text).ToList()
                        );

                    adhocGrids[adhocGridId] = 
                        grid
                        .SelectMany(col => 
                            col.Value.SelectMany((g, gridIndex) =>
                                g.Select((cell, rowIndex) =>
                                    {
                                        var id = cell.properties.Id;
                                        var hLabel = (rowIndex + 1).ToString();
                                        var vLabel = columnTitles.GetValueOrDefault(cell.properties.ScreenLeft)?[gridIndex];
                                        return (id, new Locator(hLabel: hLabel, vLabel: vLabel, gridIndex: gridIndex));
                                    }
                                )
                            )
                        )
                        .ToDictionary();
                }
                
                return adhocGrids[adhocGridId].GetValueOrDefault(component.Id);
            }

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

        enum TreeItemType 
        {
            Hierarchy,
            Image,
            Text,
            Bool,
            Button,
            Link
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
                        texts.Add(itemText.Replace("/", "//"));
                    }
                }
                return string.Join(" ", texts);
            }

            return tree.GetNodeTextByKey(nodeKey).Replace("/", "//");
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
            var itemType = (TreeItemType)tree.GetItemType(nodeKey, columnName);

            return itemType switch
            {
                TreeItemType.Text => new Locator(row: getText(tree, nodeKey), col: columnTitle),
                _ =>  new Locator(row: itemText.NullIfEmpty() ?? itemTooltip, col: columnTitle)
            };
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
                        ("DoubleClickItem", [string nodeKey, string column]) => ((GuiTree)component).GetColumnIndexFromName(column) switch
                        {
                            1 => new Locator(getTreeElementPath((GuiTree)component, nodeKey)),
                            _ => getTreeCellLocator((GuiTree)component, nodeKey, column),
                        },
                        ("DoubleClickItem" or "DoubleClickNode" or "ExpandNode", [string nodeKey]) => new Locator(getTreeElementPath((GuiTree)component, nodeKey)),
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
            var windowId = getTimestamp();

            return new Event(windowId, component.Id, componentType, locator, type, name, values);
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
            try
            {
                session.Record = false;
                Console.WriteLine("Recording stopped.");
            }
            catch (Exception) {}
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

        KeyGuiEvent? toKeyGuiEvent(List<Event> events)
        {
            if (events.Count == 0) return null;

            var component = (GuiVComponent)session.FindById(events[0].componentId);
            var componentType = events[0].componentType;
            var locator = events[0].locator;
            var lastKeyGuiEvent = keyGuiEventLog.Last();

            var keyGuiEvent = componentType switch
            {
                "GuiButton" => events switch
                {
                    [{window: long window, type: "Method", name: "Press"}] => new KeyGuiEvent(
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
                    [{window: long window, type: "Set Property", name: "Selected", values: [bool selected]}] => new KeyGuiEvent(
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
                    [{window: long window, type: "Set Property", name: "Key", values: [string value]}] => new KeyGuiEvent(
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
                    [{window: long window, type: "Method", name: "PressToolbarButton", values: [string name]}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.Push,
                        KeyGuiRoles.Button,
                        locator,
                        null
                    ),
                    [{window: long window, type: "Method", name: "ModifyCell", values: [int rowIndex, string colId, string value]}] => new KeyGuiEvent(
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
                    [{window: long window, type: "Method", name: "DoubleClickCurrentCell" }] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.DoubleClick,
                        KeyGuiRoles.Cell,
                        locator,
                        null
                    ),
                    [{window: long window, type: "Method", name: "SetCurrentCell", values: [int rowIndex, string colId] }] => new KeyGuiEvent(
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
                    [{window: long window, type: "Set Property", name: "SelectedRows", values: [string rowIndex0] }] when eventLog.SkipLast(1).Last().componentId == component.Id && eventLog.SkipLast(1).Last().name == "CurrentCellColumn" => new KeyGuiEvent(
                        window,
                        KeyGuiActions.Click,
                        KeyGuiRoles.Cell,
                        getGridViewCellLocator((GuiGridView)component, int.Parse(rowIndex0), (string)eventLog.SkipLast(1).Last().values[0]),
                        null
                    ),
                    _ => null
                },
                "GuiLabel" => events switch
                {
                    [{window: long window, name:"SetFocus"}] => new KeyGuiEvent(
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
                    [{window: long window, type: "Method", name: "SendVKey", values: [int vkey]}] => 
                        new KeyGuiEvent(
                            window,
                            KeyGuiActions.PressKey,
                            null,
                            null,
                            session.GetVKeyDescription(vkey)
                        ),
                    _ => null
                },
                "GuiOkCodeField" => events switch
                {
                    [{window: long window, type: "Set Property", name: "Text", values: [string t_code]}] =>
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
                    [{window: long window, type: "Method", name: "Select"}] => new KeyGuiEvent(
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
                    [{window: long window, type: "Method", name: "Select"}] => new KeyGuiEvent(
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
                    [{window: long window, type: "Method", name: "GetAbsoluteRow", values: [int row]},
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
                    [{window: long window, type: "Set Property", name: "Text", values: [string text]}] => new KeyGuiEvent(
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
                    [{window: long window, type: "Method", name: "DoubleClickItem"}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.DoubleClick,
                        locator!.col switch
                        {
                            string column => KeyGuiRoles.Cell,
                            null => KeyGuiRoles.TreeElement
                        },
                        locator,
                        null
                    ),
                    [{window: long window, type: "Method", name: "DoubleClickNode"}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.DoubleClick,
                        KeyGuiRoles.TreeElement,
                        locator,
                        null
                    ),
                    [{window: long window, type: "Method", name: "ExpandNode"}] => new KeyGuiEvent(
                        window,
                        KeyGuiActions.Expand,
                        KeyGuiRoles.TreeElement,
                        locator,
                        null
                    ),
                    [{window: long window, type: "Method", name: "PressButton"}] => new KeyGuiEvent(
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

            if (keyGuiEvent != null)
            {
                var window = session.ActiveWindow;
                windows.Add(new Window(keyGuiEvent.window, window.Text, getScreenshot(window, component.Id)));
            }

            return keyGuiEvent;
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

        public void saveKeyGuiLog(string filename)
        {
            saveAsJson(keyGuiEventLog, typeof(List<KeyGuiEvent>), filename + "-keygui");

            var screenshots = Path.Combine(Directory.GetCurrentDirectory(), $"{filename}-screenshots");
            Directory.CreateDirectory(screenshots);
            windows.ForEach(window => window.saveScreenshot(screenshots));
        }

        public void saveKeywordLog(string filename, string lang)
        {
            var recording = new KeywordRecording(
                keyGuiEventLog.Select(e => e.toKeywordCall(lang)).ToList(),
                windows.ToDictionary(w => w.id, w => w)
            );
            saveAsJson(recording, typeof(KeywordRecording), filename + "-keywords"); 
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
            saveAsJson(eventLog, typeof(List<Event>), filename + "-events");
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
