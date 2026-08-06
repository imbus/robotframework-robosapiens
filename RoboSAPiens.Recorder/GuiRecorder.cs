using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using sapfewse;
using saprotwr.net;
using Serilog;
using Serilog.Templates;
using NetJinja;

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

    public record RecordingStep(long window, KeywordCall keywordCall);

    public record KeywordRecording(string name, List<RecordingStep> steps, Dictionary<long, Window> windows);

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

        public KeywordCallArg toKeywordCallArg(string lang)
        {
            return new KeywordCallArg(
                name: lang switch
                {
                    "DE" => "Lokator",
                    _ => "locator"
                },
                value: ToString(),
                type: KeywordCallArgType.LOCATOR
            );
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
                _ => $"hLabel: {hLabel}, vLabel: {vLabel}, contents: {contents}, row: {row}, col: {col}, gridIndex: {gridIndex}"
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
    
    public record KeyGuiEvent(long window, string action, string? role, Locator? locator, string? value)
    {
        public override string ToString()
        {
            return $"Action: {action} | Role: {role} | Locator: {locator} | Value: {value}";
        }

        public KeywordCall toKeywordCall(string lang)
        {
            var robosapiens = new Robosapiens(lang);
            return (action, role, value) switch
            {
                (KeyGuiActions.Connect, _, string connection) => robosapiens.ConnectToServer(connection),
                (KeyGuiActions.Connect, _, null) => robosapiens.ConnectToSap(),
                (KeyGuiActions.Check, KeyGuiRoles.Cell, _) => robosapiens.TickCheckBoxCell(locator!),
                (KeyGuiActions.Check, KeyGuiRoles.Checkbox, _) => robosapiens.TickCheckBox(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.Cell, _) => robosapiens.SelectCell(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.Label, _) => robosapiens.SelectText(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.Radio, _) => robosapiens.SelectRadio(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.Tab, _) => robosapiens.SelectTab(locator!),
                (KeyGuiActions.Click, KeyGuiRoles.TextField, _) => robosapiens.SelectTextField(locator!),
                (KeyGuiActions.DoubleClick, KeyGuiRoles.Cell, _) => robosapiens.DoubleClickCell(locator!),
                (KeyGuiActions.DoubleClick, KeyGuiRoles.TextField, _) => robosapiens.DoubleClickTextField(locator!),
                (KeyGuiActions.DoubleClick, KeyGuiRoles.TreeElement, _) => robosapiens.DoubleClickTreeElement(locator!),
                (KeyGuiActions.Execute, _, string tCode) => robosapiens.ExecuteTransaction(tCode),
                (KeyGuiActions.Expand, KeyGuiRoles.TreeElement, _) => robosapiens.ExpandTreeFolder(locator!),
                (KeyGuiActions.Fill, KeyGuiRoles.Cell, string contents) => robosapiens.FillCell(locator!, contents),
                (KeyGuiActions.Fill, KeyGuiRoles.TextField, string contents) => robosapiens.FillTextField(locator!, contents),
                (KeyGuiActions.PressKey, _, string keyCombination) => robosapiens.PressKeyCombination(keyCombination),
                (KeyGuiActions.Push, KeyGuiRoles.Button, _) => robosapiens.PushButton(locator!),
                (KeyGuiActions.Push, KeyGuiRoles.Cell, _) => robosapiens.PushButtonCell(locator!),
                (KeyGuiActions.Select, KeyGuiRoles.Cell, string value) => robosapiens.SelectCellValue(locator!, value),
                (KeyGuiActions.Select, KeyGuiRoles.Combobox, string option) => robosapiens.SelectComboBox(locator!, option),
                (KeyGuiActions.Uncheck, KeyGuiRoles.Cell, _) => robosapiens.UntickCheckBoxCell(locator!),
                (KeyGuiActions.Uncheck, KeyGuiRoles.Checkbox, _) => robosapiens.UntickCheckBox(locator!),
                _ => new KeywordCall("Fail", [new KeywordCallArg("message", $"Unknown Keyword: {action} {role}", type: "ARG")])
            };
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
    
    static class LINQExtensions
    {
        public static IEnumerable<T> LogLINQ<T>(this IEnumerable<T> enumerable, string logName, bool debug)
        {
            if (debug) Log.Debug(logName + ": {@" + logName + "}", enumerable);
            return enumerable;
        }
    }

    public class NoSapException : Exception
    {
        public NoSapException(string message) : base(message) {}
    }

    record AdHocGrid(string id, List<List<SapObject>> columnTitles, Dictionary<int, List<List<SapObject>>> columns);

    public class GuiRecorder
    {
        Dictionary<string, AdHocGrid> adhocGrids = [];
        string? connectionDescription;
        bool debug;
        List<Event> eventLog = [];
        List<KeyGuiEvent> keyGuiEventLog = [];
        GuiSession? session;
        List<Window> windows = [];

        public GuiRecorder(bool debug)
        {
            this.debug = debug;

            if (debug)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(
                        outputTemplate: "{Message:lj}{NewLine}"
                    )
                    .WriteTo.PersistentFile(
                        formatter: new ExpressionTemplate("{ {@x, ..@p} }\n"),
                        path: "serilog.ndjson",
                        persistentFileRollingInterval: PersistentFileRollingInterval.Minute,
                        preserveLogFilename: true,
                        retainedFileCountLimit: 1
                    )
                    .CreateLogger();
            }
        }

        public List<KeyGuiEvent> getKeyGuiEvents()
        {
            // Create a copy of the keyGuiEventLog
            return [..keyGuiEventLog];
        }

        byte[] getScreenshot(GuiFrameWindow window, string? id)
        {
            GuiVComponent? component = null;
            byte[] screenshot;

            if (id != null)
            {
                component = (GuiVComponent)session!.FindById(id);
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

        public void addConnectEvent()
        {
            var windowId = getTimestamp();
            var window = session!.ActiveWindow;

            if (keyGuiEventLog.Count == 0)
            {
                keyGuiEventLog.Add(new KeyGuiEvent(windowId, KeyGuiActions.Connect, null, null, connectionDescription));
            }

            if (windows.Count == 0)
            {
                windows.Add(new Window(windowId, window.Text, getScreenshot(window, id: null)));
            }
        }

        object getSapGui()
        {
            var rot = new CSapROTWrapper();
            return rot.GetROTEntry("SAPGUI") ?? throw new NoSapException("SAP Logon is not running.");
        }

        GuiSession getSession()
        {
            var sapGui = getSapGui();
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
                if (session.Info.Client == "000")
                {
                    connectionDescription = connection.Description;
                }
                return session;
            }
            catch (Exception)
            {
                throw new NoSapException("Not connected to any SAP server.");
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
            if (debug) Log.Debug("component: {@component}", getSapObject(component.Id));
            var parentObject = getSapObject(component.Parent.Id);
            var verticalAlignedLabels =
                parentObject.children
                .Select(obj => obj.properties)
                .Where(obj =>
                    (obj.Type == "GuiLabel" || (obj.Type == "GuiTextField" && obj.Changeable == "false")) &&
                    Math.Abs(obj.ScreenTop - component.ScreenTop) < 5
                )
                .Where(obj => obj.Text != "");
            if (debug) Log.Debug("Vertical-aligned labels: {@verticalAlignedLabels}", verticalAlignedLabels);
            var closestLeftLabel = 
                verticalAlignedLabels
                .Where(label =>
                    label.ScreenLeft < component.ScreenLeft && 
                    Math.Abs(label.ScreenLeft + label.Width - component.ScreenLeft) < 30
                )
                .LogLINQ("LeftLabels", debug)
                .MinBy(label => Math.Abs(label.ScreenLeft + label.Width - component.ScreenLeft))
                ?.Text.Trim();
            var closestRightLabel = 
                verticalAlignedLabels
                .Where(label =>
                    label.ScreenLeft > component.ScreenLeft + component.Width && 
                    Math.Abs(label.ScreenLeft - (component.ScreenLeft + component.Width)) < 30
                )
                .MinBy(label => Math.Abs(label.ScreenLeft - (component.ScreenLeft + component.Width)))
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

        public void focusOkCode()
        {
            try
            {
                // Focus the OkCode field in order to prevent a GuiCTextField from getting the focus,
                // which changes its width due to the addition of the combo box button
                var okcd = (GuiOkCodeField)session!.ActiveWindow.FindByName("okcd", "GuiOkCodeField");
                okcd.SetFocus();
            }
            catch (Exception)
            {
            }
        }

        Locator getLocator(GuiVComponent component)
        {
            if (Regex.IsMatch(component.Id, @"\[\d+,\d+\]$", RegexOptions.Compiled))
            {
                focusOkCode();
                if (debug) Log.Debug("component: {@component}", getSapObject(component.Id));

                var adhocGridId = component.Parent.Id;

                if (!adhocGrids.ContainsKey(adhocGridId))
                {
                    var parentObject = getSapObject(adhocGridId);
                    var columns = 
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
                    if (debug) Log.Debug("columns: {@columns}, grids: {@grids}, rows: {@rows}", columns.Values.Count, columns.Values.Select(col => col.Count), columns.Values.Select(col => col.Select(grid => grid.Count)));
                    var grandParentObject = getSapObject(((GuiVComponent)component.Parent).Parent.Id);
                    var guiBox = grandParentObject.children.Find(obj => obj.properties.Type == "GuiBox");
                    var firstElement = parentObject.children.First();
                    var columnTitles = 
                        grandParentObject.children
                        .Where(obj => obj.properties.Type == "GuiLabel")
                        .Where(label => label.properties.Text != "")
                        .Where(label => label.properties.ScreenTop > guiBox!.properties.ScreenTop)
                        .Where(label => label.properties.ScreenTop < firstElement.properties.ScreenTop)
                        .GroupBy(label => label.properties.ScreenTop)
                        .Select(group => group.ToList())
                        .ToList();

                    adhocGrids[adhocGridId] = new AdHocGrid(adhocGridId, columnTitles, columns);
                }

                var adHocGrid = adhocGrids[adhocGridId];
                var cell = 
                    adHocGrid.columns[component.ScreenLeft]
                    .SelectMany((grid, gridIndex) => grid.Select((obj, rowIndex) => new {obj, rowIndex, gridIndex}))
                    .First(_ => _.obj.properties.Id == component.Id);
                if (debug) Log.Debug("cell: {@cell}", cell);
                var columnTitle = 
                    adHocGrid.columnTitles
                    .SelectMany((grid, gridIndex) => grid.Select(colTitle => new {colTitle, gridIndex}))
                    .LogLINQ("columnTitles", debug)
                    .FirstOrDefault(_ => _.gridIndex == cell.gridIndex && Math.Abs(_.colTitle.properties.ScreenLeft - component.ScreenLeft) < 4)
                    ?.colTitle.properties.Text;
                var locator = new Locator(hLabel: (cell.rowIndex + 1).ToString(), vLabel: columnTitle, gridIndex: cell.gridIndex);
                if (debug) Log.Debug("locator: {@locator}", locator);
                return locator;
            }

            return component switch
            {
                GuiButton button => new Locator(getButtonLabel(button)),
                GuiCheckBox checkBox => new Locator(checkBox.Text.Trim().NullIfEmpty() ?? getLabel(component)),
                GuiLabel label => new Locator(contents: label.Text.Trim()),
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
                "GuiOkCodeField" => null,
                "GuiMainWindow" => null,
                "GuiModalWindow" => null,
                "GuiGridView" => 
                    (name, values) switch
                    {
                        ("DoubleClickCurrentCell", _) => getGridViewCellLocator((GuiGridView)component, ((GuiGridView)component).CurrentCellRow, ((GuiGridView)component).CurrentCellColumn),
                        ("ModifyCell", [int rowIndex0, string columnId, _]) => getGridViewCellLocator((GuiGridView)component, rowIndex0, columnId),
                        ("PressToolbarButton", [string buttonId]) => new Locator(getGridViewToolbarButtonLabel((GuiGridView)component, buttonId)),
                        ("SetCurrentCell", [int rowIndex0, string columnId]) => getGridViewCellLocator((GuiGridView)component, rowIndex0, columnId),
                        _ => null
                    },
                "GuiTextField" when name == "SetFocus" =>
                    component.Parent switch
                    {
                        GuiComponent parent when parent.Type == "GuiTableControl" =>
                            ((GuiTableControl)parent).Columns switch
                            {
                                GuiCollection columns when columns.Count == 1 && ((GuiTableColumn)columns.ElementAt(0)).Title == "" => new Locator(contents: ((GuiTextField)component).Text.Trim()),
                                _ => getTableCellLocator((GuiTableControl)parent, component.Id)! with {row = ((GuiTextField)component).Text.Trim()},
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
            session = getSession();
            session.Change += handleChange;
            session.Destroy += handleDestroy;
            session.Record = true;
            Console.WriteLine("Recording started.");
        }

        public void recordStop()
        {
            try
            {
                if (session != null)
                {
                    session.Record = false;
                    Log.CloseAndFlush();
                    Console.WriteLine("Recording stopped.");
                }
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
            var objectTreeJson = session!.GetObjectTree(
                componentId,
                typeof(SapProperties).GetProperties().Select(p => p.Name).ToArray()
            );
            var objectTree = JsonNode.Parse(objectTreeJson)!.AsObject();

            return JsonSerializer.Serialize(objectTree["children"]![0], typeof(JsonObject), new SerializerContext());
        }

        KeyGuiEvent? toKeyGuiEvent(List<Event> events)
        {
            if (events.Count == 0) return null;

            var component = (GuiVComponent)session!.FindById(events[0].componentId);
            var componentType = events[0].componentType;
            var locator = events[0].locator;
            var lastKeyGuiEvent = keyGuiEventLog.LastOrDefault();

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
                        {name: "SetFocus"} when lastKeyGuiEvent?.action == KeyGuiActions.Fill && eventLog.Last().componentId == component.Id => null,
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

        public void saveHtmlReport(string name, string lang)
        {
            var curdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var bootstrap = new Dictionary<string, string>
            {
                {"css", File.ReadAllText(Path.Combine(curdir!, "bootstrap", "bootstrap.min.css"))},
                {"js",  File.ReadAllText(Path.Combine(curdir!, "bootstrap", "bootstrap.bundle.min.js"))}
            };
            var recording = getKeywordRecording(name, lang);
            var title = lang switch
            {
                "DE" => "RoboSAPiens Aufzeichnung",
                _ => "RoboSAPiens Recording"
            };
            var data = new Dictionary<string, object?>{
                {"bootstrap", bootstrap},
                {"name", recording.name},
                {"steps", recording.steps.Select(step => new Dictionary<string, object?>
                {
                    {"window", step.window},
                    {"name", step.keywordCall.name},
                    {"args", step.keywordCall.args}
                })},
                {"title", title},
                {"windows", recording.windows.Values.ToDictionary(
                    window => window.id,
                    window => new Dictionary<string, object>
                    {
                        {"id", window.id},
                        {"title", window.title},
                        {"screenshot", Convert.ToBase64String(window.screenshot)}
                    }
                )}
            };
            var filename = name.toFileName();
            var template = File.ReadAllText(Path.Combine(curdir!, "templates", "recording.jinja.html"));
            
            File.WriteAllText(
                Path.Combine(Directory.GetCurrentDirectory(), filename + ".html"),
                Jinja.Render(template, data)
            );
        }

        public void saveKeyGuiLog(string name)
        {
            var filename = name.toFileName();
            saveAsJson(keyGuiEventLog, typeof(List<KeyGuiEvent>), filename + "-keygui");

            var screenshots = Path.Combine(Directory.GetCurrentDirectory(), $"{filename}-screenshots");
            Directory.CreateDirectory(screenshots);
            windows.ForEach(window => window.saveScreenshot(screenshots));
        }

        public KeywordRecording getKeywordRecording(string name, string lang)
        {
            return new KeywordRecording(
                name,
                keyGuiEventLog.Select(e =>
                    new RecordingStep(
                        e.window,
                        e.toKeywordCall(lang)
                    )
                ).ToList(),
                windows.ToDictionary(w => w.id, w => w)
            );
        }

        public void saveKeywordLog(string name, string lang)
        {
            var recording = getKeywordRecording(name, lang);
            saveAsJson(recording, typeof(KeywordRecording), name.toFileName() + "-keywords");
        }

        public void saveEventLog(string name)
        {
            saveAsJson(eventLog, typeof(List<Event>), name.toFileName() + "-events");
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
