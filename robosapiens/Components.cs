using System;
using System.Collections.Generic;
using sapfewse;


namespace RoboSAPiens {
    public sealed class Components {
        BoxStore boxes = new BoxStore();
        ButtonStore buttons = new ButtonStore();
        ButtonStore toolbarButtons = new ButtonStore();
        CheckBoxStore checkBoxes = new CheckBoxStore();
        ComboBoxStore comboBoxes = new ComboBoxStore();
        TextFieldStore textFields = new TextFieldStore();
        TextCellStore textCells = new TextCellStore();
        LabelStore labels = new LabelStore();
        RadioButtonStore radioButtons = new RadioButtonStore();
        SAPStatusbar? statusBar = null;
        TabStore tabs = new TabStore();
        TableStore tables = new TableStore();
        GridViewStore gridViews = new GridViewStore();
        SAPTree? tree = null;
        VerticalScrollbar? verticalScrollbar = null;
        private GuiSession session;
        private bool debug;
        private string indentation = "";

        public Components(GuiSession session) {
            this.session = session;
            // Null-Object Design Pattern
        }

        public Components(GuiComponentCollection components, GuiSession session, bool debug=false) {
            this.debug = debug;
            this.session = session;

            getWindowComponents(components);

            foreach (var table in tables.getAll()) {
                classifyTableCells(table);
            }
        }

        void classifyComponent(GuiComponent component) {
            switch (component.Type) {
                case "GuiBox":
                    boxes.add(new SAPBox((GuiBox)component));
                    break;
                case "GuiButton":
                    buttons.add(new SAPButton((GuiButton)component));
                    break;
                case "GuiCheckBox":
                    checkBoxes.add(new SAPCheckBox((GuiCheckBox)component));
                    break;
                case "GuiComboBox":
                    comboBoxes.add(new SAPComboBox((GuiComboBox)component));
                    break;
                case "GuiLabel":
                    labels.add(new SAPLabel((GuiLabel)component));
                    break;
                case "GuiRadioButton":
                    radioButtons.add(new SAPRadioButton((GuiRadioButton)component));
                    break;
                case "GuiPasswordField":
                case "GuiTextField":
                case "GuiCTextField":
                    var textField = (GuiTextField)component;
                    textFields.add(new SAPTextField(textField));
                    break;
            }
        }

        void classifyContainer(GuiComponent container) {
            switch (container.Type) {
                case "GuiMenubar":
                    break;
                case "GuiShell":
                    classifyGuiShell((GuiShell)container);
                    break;
                case "GuiStatusbar":
                    statusBar = new SAPStatusbar((GuiStatusbar)container);
                    break;
                case "GuiTabStrip":
                    getTabStripComponents((GuiTabStrip)container);
                    break;
                case "GuiTableControl":
                    var sapTable = new SAPTable((GuiTableControl)container);
                    tables.add(sapTable);
                    classifyTableCells(sapTable);
                    break;
                case "GuiToolbar":
                    classifyToolbar((GuiToolbar)container);
                    break;
                case "GuiUserArea": 
                    var userArea = (GuiUserArea)container;
                    var verticalScrollbar = userArea.VerticalScrollbar;
                    if (verticalScrollbar != null && verticalScrollbar.Maximum > 0)
                    {
                        this.verticalScrollbar = new VerticalScrollbar(userArea);
                    }
                    getWindowComponents(userArea.Children);
                    break;
                default:
                    getWindowComponents(getContainerChildren(container));
                    break;
            }
        }
        
        void classifyGridViewCells(GuiGridView gridView) {
            var columnCount = gridView.ColumnCount;
            var rowCount = gridView.RowCount;
            var columnIds = (GuiCollection)gridView.ColumnOrder;

            if (debug) 
            {
                Console.WriteLine();

                for (int column = 0; column < columnCount; column++) 
                {
                    var columnId = (string)columnIds.ElementAt(column);
                    GuiCollection columnTitles = (GuiCollection)gridView.GetColumnTitles(columnId);
                    Console.Write("[");
                    for (int i = 0; i < columnTitles.Length; i++) 
                    {
                        var columnTitle = (string)columnTitles.ElementAt(i);

                        if (columnTitle.Equals(gridView.GetDisplayedColumnTitle(columnId)))
                        {
                            Console.Write($"!{columnTitle}!, ");
                        }
                        else {
                            Console.Write($"'{columnTitle}', ");
                        }
                    }
                    Console.Write("]; ");
                }

                Console.WriteLine();
            }

            for (int row = 0; row < rowCount; row++) 
            {
                if (debug) Console.WriteLine();
                if (debug) Console.Write(row + 1 + ": ");

                for (int column = 0; column < columnCount; column++) 
                {
                    var columnId = (string)columnIds.ElementAt(column);
                    var type = gridView.GetCellType(row, columnId);

                    if (debug) Console.Write(type + ", ");

                    switch (type) {
                        case "Button":
                            buttons.add(new SAPGridViewButton(columnId, gridView, row));
                            break;
                        case "CheckBox":
                            checkBoxes.add(new SAPGridViewCheckBox(columnId, gridView, row));
                            break;
                        case "Normal":
                            textCells.add(new SAPGridViewCell(columnId, gridView, row));
                            break;
                        case "ValueList":
                            comboBoxes.add(new GridViewValueList(columnId, gridView, row));
                            break;
                    }
                }

                if (debug) Console.WriteLine();
            }
        }
    
        void classifyGuiShell(GuiShell guiShell) {
            if (debug) Console.Write(": " + guiShell.SubType);

            switch (guiShell.SubType) {
                case "GridView":
                    var gridView = (GuiGridView)guiShell;
                    gridViews.add(new SAPGridView(gridView));
                    classifyGridViewToolbar(gridView);
                    classifyGridViewCells(gridView);
                    break;
                case "Toolbar":
                    classifyToolbarControl((GuiToolbarControl)guiShell);
                    break;
                case "Tree":
                    var tree = (GuiTree)guiShell;
                    this.tree = new SAPTree(tree);
                    TreeType treeType = (TreeType)tree.GetTreeType();
                    if (debug) Console.Write($" ({treeType})");
                    if (treeType == TreeType.Column || treeType == TreeType.List) {
                        classifyTreeItems(tree);
                    }
                    break;
                default:
                    getWindowComponents(guiShell.Children);
                    break;
            }
        }

        void classifyTableCells(SAPTable sapTable) {
            var table = (GuiTableControl)session.FindById(sapTable.id);
            var columns = table.Columns;
            var numRows = sapTable.getNumRows();

            for (int colIdx = 0; colIdx < columns.Length; colIdx++) {
                var column = (GuiTableColumn)columns.ElementAt(colIdx);
                var columnTitle = column.Title;

                for (int rowIdx = 0; rowIdx < numRows; rowIdx++) {
                    GuiVComponent cell;

                    // Tables are not necessarily rectangular grids
                    // A column may have a hole. Holes are skipped
                    try {
                        cell = table.GetCell(rowIdx, colIdx);
                    }
                    catch (Exception) {
                        continue;
                    }

                    var absRowIdx = sapTable.getRowsInStore() + rowIdx;
                    switch (cell.Type) {
                        case "GuiButton":
                            buttons.add(new SAPTableButton(columnTitle, absRowIdx, (GuiButton)cell, sapTable));
                            break;
                        case "GuiCheckBox":
                            checkBoxes.add(new SAPTableCheckBox(columnTitle, absRowIdx, (GuiCheckBox)cell, sapTable));
                            break;
                        case "GuiTextField":
                        case "GuiCTextField":
                            var textField = (GuiTextField)cell;
                            textCells.add(new SAPTableCell(columnTitle, absRowIdx, textField, sapTable));
                            break;
                        case "GuiComboBox":
                            var comboBox = (GuiComboBox)cell;
                            comboBoxes.add(new SAPTableComboBox(columnTitle, absRowIdx, comboBox));
                            break;
                    }
                }
            }

            sapTable.updateRowsInStore(numRows);
        }

        void classifyGridViewToolbar(GuiGridView gridView) {
            var toolbarButtonCount = gridView.ToolbarButtonCount;
            for (int i = 0; i < toolbarButtonCount; i++) {
                var type = gridView.GetToolbarButtonType(i);
                switch (type) {
                    case "Button":
                        buttons.add(new SAPGridViewToolbarButton(gridView, i));
                        break;
                    case "ButtonAndMenu":
                    case "Menu":
                        buttons.add(new SAPGridViewToolbarButtonMenu(gridView, i));
                        comboBoxes.add(new SAPGridViewToolbarButtonMenuComboBox(gridView, i));
                        break;
                    // case "CheckBox"
                }
            }
        }

        void classifyToolbarControl(GuiToolbarControl toolbar) {
            for (int i = 0; i < toolbar.ButtonCount; i++) {
                switch (toolbar.GetButtonType(i)) {
                    case "Button":
                    case "ButtonAndMenu":
                        toolbarButtons.add(new SAPToolbarButton(toolbar, i));
                        break;
                }
            }
        }

        void classifyToolbar(GuiToolbar toolbar) {
            var toolbarComponents = toolbar.Children;

            for (int i = 0; i < toolbarComponents.Length; i++)
            {
                var component = toolbarComponents.ElementAt(i);
                switch (component.Type)
                {
                    case "GuiButton":
                        toolbarButtons.add(new SAPButton((GuiButton)component));
                        break;
                }
            }
        }

        // TODO: The Tree class should take care of classifying its elements
        void classifyTreeItems(GuiTree tree) {
            var columnNames = (GuiCollection)tree.GetColumnNames();

            if (columnNames != null)
            {
                var paths = SAPTree.getAllPaths(tree);

                if (debug) Console.WriteLine();

                for (int i = 0; i < columnNames.Length; i++) 
                {
                    var columnName = (string)columnNames.ElementAt(i);
                    if (columnName == null) { continue; }

                    string columnTitle;
                    try {
                        columnTitle = tree.GetColumnTitleFromName(columnName);
                        if (debug) Console.WriteLine(columnTitle);
                    }
                    catch (Exception) {
                        continue;
                    }

                    for (int index = 0; index < paths.Count; index++) 
                    {
                        var nodePath = paths[index];
                        var nodeKey = tree.GetNodeKeyByPath(nodePath);

                        TreeItem itemType = (TreeItem)tree.GetItemType(nodeKey, columnName);
                        if (itemType == TreeItem.Hierarchy) continue;

                        var itemText = tree.GetItemText(nodeKey, columnName);
                        var itemTooltip = tree.GetItemToolTip(nodeKey, columnName);

                        if (debug) Console.WriteLine($"{nodePath}: ({itemText}, {itemTooltip}) [{itemType}]");

                        switch (itemType) 
                        {
                            case TreeItem.Bool:
                                checkBoxes.add(new SAPTreeCheckBox(columnName, columnTitle, nodeKey, rowNumber: index, tree.Id));
                                break;
                            case TreeItem.Button:
                                buttons.add(new SAPTreeButton(columnName, columnTitle, itemText, nodeKey, rowNumber: index, tree.Id));
                                break;
                            case TreeItem.Link:
                                buttons.add(new SAPTreeLink(columnName, columnTitle, itemText, itemTooltip, nodeKey, rowNumber: index, tree.Id));
                                textCells.add(new SAPTreeCell(columnName, columnTitle, rowIndex: index, content: itemText, nodeKey, tree));
                                break;
                            case TreeItem.Text:
                                textCells.add(new SAPTreeCell(columnName, columnTitle, rowIndex: index, content: itemText, nodeKey, tree));
                                break;
                        }
                    }
                }
            }
        }

        GuiComponentCollection getContainerChildren(GuiComponent container) {
            return container.Type switch {
                "GuiContainerShell" => ((GuiContainerShell)container).Children,
                "GuiCustomControl" => ((GuiCustomControl)container).Children,
                "GuiDialogShell" => ((GuiDialogShell)container).Children,
                "GuiDockShell" => ((GuiContainerShell)container).Children,
                "GuiGOSShell" => ((GuiGOSShell)container).Children,
                "GuiMenu" => ((GuiMenu)container).Children,
                "GuiScrollContainer" => ((GuiScrollContainer)container).Children,
                "GuiSimpleContainer" => ((GuiSimpleContainer)container).Children,
                "GuiSplitterContainer" => ((GuiSplitterContainer)container).Children,
                "GuiSplitterShell" => ((GuiSplit)container).Children,
                "GuiTab" => ((GuiTab)container).Children,
                "GuiTitlebar" => ((GuiTitlebar)container).Children,
                "GuiToolbar" => ((GuiToolbar)container).Children,
                _ => ((GuiContainer)container).Children
            };
        }

        public enum TreeItem {
            Hierarchy,
            Image,
            Text,
            Bool,
            Button,
            Link,
        }

        public enum TreeType {
            Simple,
            List,
            Column
        }

        void getTabStripComponents(GuiTabStrip tabStrip)
        {
            var sapTabs = tabStrip.Children;

            if (debug)
            {
                Console.Write(" | ");
                for (int i = 0; i < sapTabs.Length; i++) 
                {
                    var tab = (GuiTab)sapTabs.ElementAt(i);
                    Console.Write(tab.Text + " | ");
                } 
            }

            for (int i = 0; i < sapTabs.Length; i++)
            {
                var tab = (GuiTab)sapTabs.ElementAt(i);
                tabs.add(new SAPTab(tab));
                getWindowComponents(tab.Children);
            }
        }

        void getWindowComponents(GuiComponentCollection components) {
            var localIndentation = indentation;
            indentation += "|  ";

            for (int i = 0; i < components.Length; i++) {
                var component = (GuiComponent)components.ElementAt(i);
                if (debug) Console.WriteLine();
                if (debug) Console.Write(localIndentation + component.Type);

                if (component.ContainerType) {
                    classifyContainer(component);
                }
                else {
                    classifyComponent(component);
                }
            }

            indentation = localIndentation;
        }

        public Button? findButton(ButtonLocator button) {
            return buttons.get(button.locator) ?? toolbarButtons.get(button.locator);
        }

        public Button? findButtonCell(CellLocator locator) {
            return buttons.get(locator, textCells);
        }

        public CheckBox? findCheckBox(CheckBoxLocator checkBox) {
            return checkBoxes.get(checkBox.locator, labels, textFields);
        }

        public CheckBox? findCheckBoxCell(CellLocator locator) {
            return checkBoxes.get(locator, textCells);
        }

        public ComboBox? findComboBox(ComboBoxLocator comboBox) {
            return comboBoxes.get(comboBox.locator, labels, textFields);
        }

        public ComboBox? findComboBoxCell(CellLocator locator) {
            return comboBoxes.getCell(locator, textCells);
        }

        public IHighlightable? findHighlightableButton(ButtonLocator button) {
            return buttons.get(button.locator) switch {
                Button b when b is IHighlightable => b as IHighlightable,
                _ => null
            };
        }

        public ITextElement? findLabel(LabelLocator labelLocator) {
            return labels.get(labelLocator.locator, labels, textFields) as ITextElement ??
                   textCells.get(labelLocator.locator);
        }

        public RadioButton? findRadioButton(RadioButtonLocator radioButton) {
            return radioButtons.get(radioButton.locator, labels, textFields);
        }

        public SAPTab? findTab(string tabName) {
            return tabs.get(tabName);
        }

        public TextCell? findTextCell(CellLocator locator) {
            return textCells.get(locator);
        }

        public SAPTextField? findTextField(TextFieldLocator textField) {
            return textFields.get(textField.locator, labels, boxes);
        }

        public List<SAPButton> getAllButtons() {
            return new List<SAPButton>(buttons.filterBy<SAPButton>());
        }

        public List<SAPLabel> getAllLabels() {
            return labels.getAll();
        }

        public List<SAPTableCell> getAllTableCells() {
            return new List<SAPTableCell>(textCells.filterBy<SAPTableCell>());
        }

        public List<SAPGridViewCell> getAllGridViewCells() {
            return new List<SAPGridViewCell>(textCells.filterBy<SAPGridViewCell>());
        }

        public List<SAPTextField> getAllTextFields() {
            return new List<SAPTextField>(textFields.getAll());
        }

        public SAPStatusbar? getStatusBar() {
            return statusBar;
        }

        public List<SAPGridView> getGridViews() {
            return gridViews.getAll();
        }

        public VerticalScrollbar? getVerticalScrollbar()
        {
            return verticalScrollbar;
        }

        public List<SAPTable> getTables(){
            return tables.getAll();
        }

        public SAPTree? getTree() {
            return tree;
        }
    }

    public record FormField(string Text, string Label, string Id, int Left, int Top, int Width, int Height);
}
