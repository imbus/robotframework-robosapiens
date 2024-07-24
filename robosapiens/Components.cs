using System;
using System.Collections.Generic;
using System.Linq;
using sapfewse;


namespace RoboSAPiens {
    public sealed class Components 
    {
        BoxStore boxes = new BoxStore();
        ButtonStore buttons = new ButtonStore();
        CheckBoxStore checkBoxes = new CheckBoxStore();
        CellRepository cellRepository = new CellRepository();
        ComboBoxStore comboBoxes = new ComboBoxStore();
        GridViewStore gridViews = new GridViewStore();
        LabelStore labels = new LabelStore();
        MenuItemStore menuItems = new MenuItemStore();
        RadioButtonStore radioButtons = new RadioButtonStore();
        SAPStatusbar? statusBar = null;
        TabStore tabs = new TabStore();
        TableStore tables = new TableStore();
        TextFieldStore textFields = new TextFieldStore();
        ButtonStore toolbarButtons = new ButtonStore();
        TreeStore trees = new TreeStore();
        HorizontalScrollbar? horizontalScrollbar = null;
        VerticalScrollbar? verticalScrollbar = null;
        private bool debug;
        private string indentation = "";

        public Components(GuiComponentCollection components, bool debug=false) {
            this.debug = debug;

            getWindowComponents(components);
        }

        void getWindowComponents(GuiComponentCollection components)
        {
            var localIndentation = indentation;
            indentation += "|  ";

            for (int i = 0; i < components.Length; i++) 
            {
                var component = components.ElementAt(i);
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

        void classifyComponent(GuiComponent component)
        {
            switch (component.Type) 
            {
                case "GuiBox":
                    boxes.Add(new SAPBox((GuiBox)component));
                    break;
                case "GuiButton":
                    buttons.Add(new SAPButton((GuiButton)component));
                    break;
                case "GuiCheckBox":
                    checkBoxes.Add(new SAPCheckBox((GuiCheckBox)component));
                    break;
                case "GuiComboBox":
                    comboBoxes.Add(new SAPComboBox((GuiComboBox)component));
                    break;
                case "GuiLabel":
                    labels.Add(new SAPLabel((GuiLabel)component));
                    break;
                case "GuiRadioButton":
                    radioButtons.Add(new SAPRadioButton((GuiRadioButton)component));
                    break;
                case "GuiPasswordField":
                case "GuiTextField":
                case "GuiCTextField":
                    var textField = (GuiTextField)component;
                    textFields.Add(new SAPTextField(textField));
                    break;
            }
        }

        void classifyContainer(GuiComponent container)
        {
            switch (container.Type) 
            {
                case "GuiMenubar":
                    classifyMenubar((GuiMenubar)container);
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
                    var guiTableControl = (GuiTableControl)container;
                    var sapTable = new SAPTable(guiTableControl);
                    tables.Add(sapTable);
                    if (debug) sapTable.print(guiTableControl);
                    break;
                case "GuiToolbar":
                    classifyToolbar((GuiToolbar)container);
                    break;
                case "GuiUserArea": 
                    var userArea = (GuiUserArea)container;
                    var horizontalScrollbar = userArea.HorizontalScrollbar;
                    if (horizontalScrollbar != null && horizontalScrollbar.Maximum > 0) {
                        this.horizontalScrollbar = new HorizontalScrollbar(userArea);
                    }
                    var verticalScrollbar = userArea.VerticalScrollbar;
                    if (verticalScrollbar != null && verticalScrollbar.Maximum > 0) {
                        this.verticalScrollbar = new VerticalScrollbar(userArea);
                    }
                    getWindowComponents(userArea.Children);
                    break;
                default:
                    getWindowComponents(getContainerChildren(container));
                    break;
            }
        }

        void classifyGuiShell(GuiShell guiShell)
        {
            if (debug) Console.Write(": " + guiShell.SubType);

            switch (guiShell.SubType) 
            {
                case "GridView":
                    var gridView = (GuiGridView)guiShell;
                    var sapGridView = new SAPGridView(gridView);
                    gridViews.Add(new SAPGridView(gridView));
                    classifyGridViewToolbar(gridView);
                    if (debug) sapGridView.print(gridView);
                    break;
                case "Toolbar":
                    classifyToolbarControl((GuiToolbarControl)guiShell);
                    break;
                case "Tree":
                    var guiTree = (GuiTree)guiShell;
                    var sapTree = new SAPTree(guiTree);
                    this.trees.Add(sapTree);
                    if (debug) sapTree.print(guiTree);
                    break;
                default:
                    getWindowComponents(guiShell.Children);
                    break;
            }
        }

        void processSubmenu(GuiMenu guiMenu, string parent)
        {
            var guiMenus = guiMenu.Children;

            for (int i = 0; i < guiMenus.Count; i++)
            {
                var menu = (GuiMenu)guiMenus.ElementAt(i);
                var path = $"{parent}/{menu.Text}";

                if (menu.Text != "") {
                    menuItems.Add(new SAPMenu(menu, path));
                }

                if (menu.Children.Count > 0) {
                    processSubmenu(menu, path);
                }

            }
        }

        void classifyMenubar(GuiMenubar guiMenubar)
        {
            var guiMenus = guiMenubar.Children;

            for (int i = 0; i < guiMenus.Count; i++)
            {
                var menu = (GuiMenu)guiMenus.ElementAt(i);
                
                if (menu.Text != "") {
                    menuItems.Add(new SAPMenu(menu, menu.Text));
                }
                
                if (menu.Children.Count > 0) {
                    processSubmenu(menu, menu.Text);
                }
            }
        }

        void classifyToolbarControl(GuiToolbarControl toolbar)
        {
            for (int i = 0; i < toolbar.ButtonCount; i++) {
                switch (toolbar.GetButtonType(i)) {
                    case "Button":
                    case "ButtonAndMenu":
                    case "CheckBox":
                        toolbarButtons.Add(new SAPToolbarButton(toolbar, i));
                        break;
                    case "Menu":
                        comboBoxes.Add(new SAPToolbarMenu(toolbar, i));
                        break;                    
                }
            }
        }

        void classifyGridViewToolbar(GuiGridView gridView)
        {
            for (int i = 0; i < gridView.ToolbarButtonCount; i++) 
            {
                var type = gridView.GetToolbarButtonType(i);
                switch (type) {
                    case "Button":
                        buttons.Add(new SAPGridViewToolbarButton(gridView, i));
                        break;
                    case "ButtonAndMenu":
                    case "Menu":
                        buttons.Add(new SAPGridViewToolbarButtonMenu(gridView, i));
                        comboBoxes.Add(new SAPGridViewToolbarButtonMenuComboBox(gridView, i));
                        break;
                    // case "CheckBox"
                }
            }
        }

        void classifyToolbar(GuiToolbar toolbar)
        {
            var toolbarComponents = toolbar.Children;

            for (int i = 0; i < toolbarComponents.Length; i++)
            {
                var component = toolbarComponents.ElementAt(i);
                switch (component.Type)
                {
                    case "GuiButton":
                        toolbarButtons.Add(new SAPButton((GuiButton)component));
                        break;
                }
            }
        }

        GuiComponentCollection getContainerChildren(GuiComponent container)
        {
            return container.Type switch 
            {
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
                tabs.Add(new SAPTab(tab));
                getWindowComponents(tab.Children);
            }
        }

        public void updateTables(GuiSession session)
        {
            tables.ForEach(table => table.classifyCells(session, cellRepository));
            gridViews.ForEach(gridView => gridView.classifyCells(session, cellRepository));
            trees.ForEach(tree => tree.classifyCells(session, cellRepository));
        }

        public Button? findButton(ButtonLocator button) {
            return buttons.get(button.locator,  labels, textFields) ?? 
                   toolbarButtons.get(button.locator,  labels, textFields);
        }

        public CheckBox? findCheckBox(CheckBoxLocator checkBox) {
            return checkBoxes.get(checkBox.locator, labels, textFields);
        }

        public ComboBox? findComboBox(ComboBoxLocator comboBox) {
            return comboBoxes.get(comboBox.locator, labels, textFields);
        }

        public ITextElement? findLabel(LabelLocator labelLocator) {
            return labels.get(labelLocator.locator, labels, textFields);
        }

        public SAPMenu? findMenuItem(String itemPath) {
            return menuItems.get(itemPath);
        }

        public RadioButton? findRadioButton(RadioButtonLocator radioButton) {
            return radioButtons.get(radioButton.locator, labels, textFields);
        }

        public SAPTab? findTab(string tabName) {
            return tabs.get(tabName);
        }

        public SAPTextField? findTextField(TextFieldLocator textField) {
            return textFields.get(textField.locator, labels, boxes);
        }

        public Button? findButtonCell(CellLocator locator, GuiSession session) 
        {
            if (cellRepository.isEmpty()) {
                updateTables(session);
            }

            return cellRepository.findButtonCell(locator);
        }

        public CheckBox? findCheckBoxCell(CellLocator locator, GuiSession session) 
        {
            if (cellRepository.isEmpty()) {
                updateTables(session);
            }

            return cellRepository.findCheckBoxCell(locator);
        }

        public ComboBox? findComboBoxCell(CellLocator locator, GuiSession session) 
        {
            if (cellRepository.isEmpty()) {
                updateTables(session);
            }

            return cellRepository.findComboBoxCell(locator);
        }

        public TextCell? findTextCell(ILocator locator, GuiSession session) 
        {
            if (cellRepository.isEmpty()) {
                updateTables(session);
            }

            return cellRepository.findTextCell(locator);
        }
        
        public SAPTreeElement? findTreeElement(string elementPath, GuiSession session) {
            if (cellRepository.isEmpty()) {
                updateTables(session);
            }
            
            return getTree()?.findTreeElement(elementPath);
        }

        public List<SAPButton> getAllButtons() {
            return new List<SAPButton>(buttons.filterBy<SAPButton>());
        }

        public List<SAPLabel> getAllLabels() {
            return labels;
        }

        public List<SAPTextField> getAllTextFields() {
            return textFields;
        }

        public SAPStatusbar? getStatusBar() {
            return statusBar;
        }

        public void setStatusBar(GuiStatusbar guiStatusbar) {
            statusBar = new SAPStatusbar(guiStatusbar);
        }

        public List<SAPGridView> getGridViews() {
            return gridViews;
        }

        public HorizontalScrollbar? getHorizontalScrollbar() {
            return horizontalScrollbar;
        }

        public VerticalScrollbar? getVerticalScrollbar() {
            return verticalScrollbar;
        }

        public ITable? getFirstTable()
        {
            return tables.FirstOrDefault() as ITable ??
                   gridViews.FirstOrDefault() as ITable ??
                   trees.FirstOrDefault();
        }

        public SAPTree? getTree() {
            return trees.FirstOrDefault();
        }
    }

    public record FormField(string Text, string Label, string Id, int Left, int Top, int Width, int Height);
}
