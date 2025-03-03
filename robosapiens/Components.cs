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
        ComboBoxStore comboBoxes = new ComboBoxStore();
        LabelStore labels = new LabelStore();
        MenuItemStore menuItems = new MenuItemStore();
        RadioButtonStore radioButtons = new RadioButtonStore();
        SAPStatusbar? statusBar = null;
        SAPTextEdit? textEdit = null;
        TabStore tabs = new TabStore();
        TableStore tables = new TableStore();
        TextFieldStore textFields = new TextFieldStore();
        ButtonStore toolbarButtons = new ButtonStore();
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
                    sapTable.getTextFields(guiTableControl, textFields);
                    tables.Add(sapTable);
                    if (debug) sapTable.print();
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
                    tables.Add(new SAPGridView(gridView));
                    classifyGridViewToolbar(gridView);
                    if (debug) sapGridView.print();
                    break;
                case "Toolbar":
                    classifyToolbarControl((GuiToolbarControl)guiShell);
                    break;
                case "Tree":
                    var guiTree = (GuiTree)guiShell;
                    var sapTree = new SAPTree(guiTree);
                    this.tables.Add(sapTree);
                    if (debug) sapTree.print();
                    break;
                case "TextEdit":
                    textEdit = new SAPTextEdit((GuiTextedit)guiShell);
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

        public Button? findButton(ButtonLocator button) {
            return buttons.get(button.locator, labels, textFields.NonChangeable()) ?? 
                   toolbarButtons.get(button.locator,  labels, textFields.NonChangeable());
        }

        public CheckBox? findCheckBox(CheckBoxLocator checkBox) {
            return checkBoxes.get(checkBox.locator, labels, textFields.NonChangeable());
        }

        public ComboBox? findComboBox(ComboBoxLocator comboBox) {
            return comboBoxes.get(comboBox.locator, labels, textFields.NonChangeable());
        }

        public ITextElement? findLabel(LabelLocator labelLocator) {
            return labels.get(labelLocator.locator, labels, textFields.NonChangeable());
        }

        public SAPMenu? findMenuItem(String itemPath) {
            return menuItems.get(itemPath);
        }

        public RadioButton? findRadioButton(RadioButtonLocator radioButton) {
            return radioButtons.get(radioButton.locator, labels, textFields.NonChangeable());
        }

        public SAPTab? findTab(string tabName) {
            return tabs.get(tabName);
        }

        public SAPTextField? findTextField(TextFieldLocator textField) {
            return textFields.get(textField.locator, labels, boxes);
        }
        
        public Cell? findCell(ILocator locator, GuiSession session, int? tableNumber)
        {
            var tables = getTables();

            if (tableNumber != null) 
            {
                var table = tables[(int)tableNumber-1];
                return table.findCell(locator, session);
            }
            else
            {
                foreach (var table in tables)
                {
                    var cell = table.findCell(locator, session);
                    if (cell != null) return cell;
                }
            }

            return null;
        }

        public SAPTreeElement? findTreeElement(string elementPath, GuiSession session)
        {
            foreach (var tree in getTrees())
            {
                var treeElement = tree.findTreeElement(elementPath, session);
                if (treeElement != null) return treeElement;
            }
            
            return null;
        }

        public List<SAPButton> getAllButtons() {
            return buttons.filterBy<SAPButton>().ToList();
        }

        public List<SAPLabel> getAllLabels() {
            return labels;
        }

        public List<SAPTextField> getAllTextFields() {
            return textFields.GetAll();
        }

        public SAPStatusbar? getStatusBar() {
            return statusBar;
        }

        public SAPTextEdit? getTextEdit() {
            return textEdit;
        }

        public void setStatusBar(GuiStatusbar guiStatusbar) {
            statusBar = new SAPStatusbar(guiStatusbar);
        }

        public List<SAPGridView> getGridViews() {
            return tables.filterBy<SAPGridView>();
        }

        public HorizontalScrollbar? getHorizontalScrollbar() {
            return horizontalScrollbar;
        }

        public VerticalScrollbar? getVerticalScrollbar() {
            return verticalScrollbar;
        }

        public List<ITable> getTables()
        {
            return tables;
        }

        public List<SAPTree> getTrees() {
            return tables.filterBy<SAPTree>();
        }
    }

    public record FormField(string Text, string Label, string Id, int Left, int Top, int Width, int Height);
}
