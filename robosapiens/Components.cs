using System;
using System.Collections.Generic;
using System.Linq;
using sapfewse;


namespace RoboSAPiens {
    public sealed class Components {
        BoxStore boxes = new BoxStore();
        ButtonStore buttons = new ButtonStore();
        ButtonStore toolbarButtons = new ButtonStore();
        CheckBoxStore checkBoxes = new CheckBoxStore();
        ComboBoxStore comboBoxes = new ComboBoxStore();
        EditableCellStore editableCells = new EditableCellStore();
        EditableTextFieldStore editableTextFields = new EditableTextFieldStore();
        LabelCellStore labelCells = new LabelCellStore();
        LabelStore labels = new LabelStore();
        RadioButtonStore radioButtons = new RadioButtonStore();
        ReadOnlyTextFieldStore readOnlyTextFields = new ReadOnlyTextFieldStore();
        SAPStatusbar? statusBar = null;
        TabStore tabs = new TabStore();
        TableStore tables = new TableStore();
        GridViewStore gridViews = new GridViewStore();
        SAPTree? tree = null;
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


                if (table.rowsAreMissing()) {
                    throw new Exception("The table does not fit in the window. " +
                    "Maximize the window and make sure that the screen resolution is at least 1920x1080 and the scaling is 100%");

                    // Commented out because after scrolling the table cells can become unchangeable
                    // while (table.rowsAreMissing()) {
                    //     table.scrollOnePage(session);
                    // }
                    // table.scrollToTop(session);
                }
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

                    if (textField.Changeable) {
                        editableTextFields.add(new EditableTextField(textField));
                    } 
                    else {
                        readOnlyTextFields.add(new SAPTextField(textField));
                    }

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
                    var columnTitle = gridView.GetDisplayedColumnTitle(columnId);
                    Console.Write(columnTitle + ", ");
                }

                Console.WriteLine();
            }

            for (int row = 0; row < rowCount; row++) 
            {
                if (debug) Console.Write(row + ": ");

                for (int column = 0; column < columnCount; column++) 
                {
                    var columnId = (string)columnIds.ElementAt(column);
                    var type = gridView.GetCellType(row, columnId);

                    if (debug) Console.Write(type + ", ");

                    switch (type) {
                        case "Normal":
                            if (gridView.GetCellChangeable(row, columnId)) {
                                editableCells.add(new EmptyGridViewCell(columnId, gridView, row));
                            } else {
                                labelCells.add(new SAPGridViewCell(columnId, gridView, row));
                            }
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
                    if ((TreeType)tree.GetTreeType() == TreeType.Column) {
                        classifyTreeComponents(tree);
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
                            if (textField.Changeable) {
                                editableCells.add(new EditableTableCell(columnTitle, absRowIdx, textField, sapTable));
                            } 
                            else {
                                labelCells.add(new SAPTableCell(columnTitle, absRowIdx, textField, sapTable));
                            }
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
                        var id = gridView.GetToolbarButtonId(i);
                        var tooltip = gridView.GetToolbarButtonTooltip(i);
                        buttons.add(new SAPGridViewToolbarButton(gridView, id, tooltip));
                        break;
                    // case "ButtonAndMenu"
                    // case "CheckBox"
                    // case "Menu"
                }
            }
        }

        void classifyToolbarControl(GuiToolbarControl toolbar) {
            for (int i = 0; i < toolbar.ButtonCount; i++) {
                switch (toolbar.GetButtonType(i)) {
                    case "Button":
                        var id = toolbar.GetButtonId(i);
                        var tooltip = toolbar.GetButtonTooltip(i);
                        toolbarButtons.add(new SAPToolbarButton(toolbar, id, tooltip));
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
        void classifyTreeComponents(GuiTree tree) {
            var columnNames = (GuiCollection)tree.GetColumnNames();
            var paths = SAPTree.getAllPaths(tree);

            for (int i = 0; i < columnNames.Length; i++) {
                var columnName = (string)columnNames.ElementAt(i);
                if (columnName == null) { continue; }
                var columnTitle = tree.GetColumnTitleFromName(columnName);

                for (int index = 0; index < paths.Count; index++) {
                    var nodePath = paths[index];
                    var nodeKey = tree.GetNodeKeyByPath(nodePath);
                    var itemText = tree.GetItemText(nodeKey, columnName);
                    var itemType = (TreeItem)tree.GetItemType(nodeKey, columnName);

                    switch (itemType) {
                        case TreeItem.Bool:
                            checkBoxes.add(new SAPTreeCheckBox(columnName, columnTitle, nodeKey, rowNumber: index, tree.Id));
                            break;
                        case TreeItem.Button:
                            buttons.add(new SAPTreeButton(columnName, columnTitle, itemText, nodeKey, rowNumber: index, tree.Id));
                            break;
                        case TreeItem.Link:
                            var tooltip = tree.GetItemToolTip(nodeKey, columnName);
                            buttons.add(new SAPTreeLink(columnName, columnTitle, tooltip, nodeKey, tree.Id));
                            break;
                        case TreeItem.Text:
                            labelCells.add(new SAPTreeCell(columnName, columnTitle, rowIndex: index, content: itemText, nodeKey, tree));
                            break;
                    }
                }
            }
        }

        GuiComponentCollection getContainerChildren(GuiComponent container) {
            return container.Type switch {
                "GuiContainerShell" => ((GuiContainerShell)container).Children,
                "GuiDockShell" => ((GuiContainerShell)container).Children,
                "GuiCustomControl" => ((GuiCustomControl)container).Children,
                "GuiGOSShell" => ((GuiGOSShell)container).Children,
                "GuiMenu" => ((GuiMenu)container).Children,
                "GuiScrollContainer" => ((GuiScrollContainer)container).Children,
                "GuiSimpleContainer" => ((GuiSimpleContainer)container).Children,
                "GuiSplitterShell" => ((GuiSplit)container).Children,
                "GuiTab" => ((GuiTab)container).Children,
                "GuiTitlebar" => ((GuiTitlebar)container).Children,
                "GuiToolbar" => ((GuiToolbar)container).Children,
                "GuiUserArea" => ((GuiUserArea)container).Children,
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

        void getTabStripComponents(GuiTabStrip tabStrip) {
            var sapTabs = tabStrip.Children;
            for (int i = 0; i < sapTabs.Length; i++) {
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

        public Button? findButtonCell(FilledCellLocator cell) {
            return buttons.get(cell);
        }

        public CheckBox? findCheckBox(CheckBoxLocator checkBox) {
            return checkBoxes.get(checkBox.locator, labels, readOnlyTextFields);
        }

        public CheckBox? findCheckBoxCell(CellLocator locator) {
            return checkBoxes.get(locator, labels, readOnlyTextFields);
        }

        public ComboBox? findComboBox(ComboBoxLocator comboBox) {
            return comboBoxes.get(comboBox.locator);
        }

        public Cell? findCell(CellLocator locator) {
            return editableCells.get(locator, labelCells) ?? 
                   labelCells.get(locator);
        }

        public IDoubleClickable? findDoubleClickableCell(CellLocator locator) {
            var cell = editableCells.get(locator, labelCells) ??
                       labelCells.get(locator);

            if  (cell is IDoubleClickable) {
                return cell as IDoubleClickable;
            }

            return null;
        }

        public IEditableCell? findEditableCell(CellLocator locator) {
            return editableCells.get(locator, labelCells) as IEditableCell;
        }

        public EditableTextField? findEditableTextField(TextFieldLocator textField) {
            return editableTextFields.get(textField.locator, labels, readOnlyTextFields, boxes);
        }

        public IHighlightable? findHighlightableButton(ButtonLocator button) {
            return buttons.get(button.locator) switch {
                Button b when b is IHighlightable => b as IHighlightable,
                _ => null
            };
        }

        public RadioButton? findRadioButton(RadioButtonLocator radioButton) {
            return radioButtons.get(radioButton.locator, labels, readOnlyTextFields);
        }
        
        public SAPTextField? findReadOnlyTextField(TextFieldLocator textField) {
            return readOnlyTextFields.get(textField.locator, labels, readOnlyTextFields, boxes);
        }

        public SAPTab? findTab(string tabName) {
            return tabs.get(tabName);
        }

        public SAPTextField? findTextField(TextFieldLocator textField) {
            return findEditableTextField(textField) ?? 
                   findReadOnlyTextField(textField);
        }

        public ITextElement? findLabel(LabelLocator content) {
            return labels.get(content.locator, labels, readOnlyTextFields) as ITextElement ??
                   labelCells.get(content.locator);
        }

        public List<SAPButton> getAllButtons() {
            return new List<SAPButton>(buttons.filterBy<SAPButton>());
        }

        public List<SAPLabel> getAllLabels() {
            return labels.getAll();
        }

        public List<SAPTableCell> getAllTableCells() {
            return new List<SAPTableCell>(labelCells.filterBy<SAPTableCell>().Concat(editableCells.filterBy<SAPTableCell>()));
        }

        public List<SAPGridViewCell> getAllGridViewCells() {
            return new List<SAPGridViewCell>(labelCells.filterBy<SAPGridViewCell>().Concat(editableCells.filterBy<SAPGridViewCell>()));
        }

        public List<SAPTextField> getAllTextFields() {
            return new List<SAPTextField>(readOnlyTextFields.getAll().Concat(editableTextFields.getAll()));
        }

        public SAPStatusbar? getStatusBar() {
            return statusBar;
        }

        public List<SAPGridView> getGridViews() {
            return gridViews.getAll();
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
