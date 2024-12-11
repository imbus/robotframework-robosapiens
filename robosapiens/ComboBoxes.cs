using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens {
    public abstract class ComboBox: ITextElement {
        protected bool focused;
        public abstract bool contains(string entry);

        public abstract string getText(GuiSession session);

        public abstract void select(string entry, GuiSession session);

        public void select(GuiSession session) {}

        public abstract void toggleHighlight(GuiSession session);
    }

    public class SAPComboBox: ComboBox, ILabeled, ILocatable {
        string accTooltip;
        List<string> entries;
        string id;
        string label;
        Position position;

        public SAPComboBox(GuiComboBox comboBox) {
            accTooltip = comboBox.AccTooltip.Trim();
            entries = new List<string>();
            id = comboBox.Id;
            label = getLeftLabel(comboBox);
            getEntries(comboBox);
            this.position = new Position(height: comboBox.Height, 
                                         left: comboBox.ScreenLeft,
                                         top: comboBox.ScreenTop, 
                                         width: comboBox.Width);
        }

        public List<string> getLabels()
        {
            return new List<string>
            {
                accTooltip
            };
        }

        void getEntries(GuiComboBox comboBox) {
            var entries = comboBox.Entries;
            for (int i = 0; i < entries.Length; i++) {
                var comboBoxEntry = (GuiComboBoxEntry)entries.ElementAt(i);
                this.entries.Add(comboBoxEntry.Value);
            }
        }

        // To Do: Factor out this function for ComboBox and TextField
        string getLeftLabel(GuiComboBox comboBox) {
            var leftLabel = (GuiComponent)comboBox.LeftLabel;
            if (leftLabel != null) {
                if (leftLabel.Type == "GuiLabel") {
                    var label = (GuiLabel)leftLabel;
                    return label.Text;
                }
                if (leftLabel.Type == "GuiTextField") {
                    var label = (GuiTextField)leftLabel;
                    return label.Text;
                }
            }
            return "";
        }

        public override bool contains(string query) {
            var result = entries.Find(entry => entry.Equals(query));
            return result != null;
        }

        public Position getPosition() {
            return position;
        }

        public bool isHLabeled(string label) {
            return this.label == label;
        }

        public bool isVLabeled(string label) {
            return false;
        }

        public bool isLocated(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels) {
            return locator switch {
                HLabel(var label) =>
                    isHorizontalAlignedWithLabel(labels.getByName(label)) ||
                    isHorizontalAlignedWithTextField(textFieldLabels.getByContent(label)),
                _ => false
            };
        }

        public bool isHorizontalAlignedWithLabel(SAPLabel? label) {
            return label switch {
                SAPLabel => label.position.horizontalAlignedWith(position),
                _ => false
            };
        }

        public bool isHorizontalAlignedWithTextField(SAPTextField? textField) {
            return textField switch {
                SAPTextField => textField.position.horizontalAlignedWith(position),
                _ => false
            };
        }

        public bool hasTooltip(string tooltip) {
            return accTooltip == tooltip;
        }

        public override void select(string entry, GuiSession session)
        {
            var guiComboBox = (GuiComboBox)session.FindById(id);
            guiComboBox.Value = entry;
        }

        public override void toggleHighlight(GuiSession session) {
            focused = !focused;
            var guiComboBox = (GuiComboBox)session.FindById(id);
            guiComboBox.Visualize(focused);
        }

        public override string getText(GuiSession session)
        {
            var guiComboBox = (GuiComboBox)session.FindById(id);
            return guiComboBox.Value;
        }
    }

    public sealed class SAPGridViewToolbarButtonMenuComboBox: ComboBox, ILabeled 
    {
        string gridViewId;
        string tooltip;

        public SAPGridViewToolbarButtonMenuComboBox(GuiGridView gridView, int position) 
        {
            this.gridViewId = gridView.Id;
            this.tooltip = gridView.GetToolbarButtonTooltip(position);
        }

        public override bool contains(string entry)
        {
            return true;
        }

        public bool isHLabeled(string label) {
            return false;
        }

        public bool isVLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) 
        {
            return this.tooltip == tooltip;
        }

        public override void select(string entry, GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.SelectContextMenuItemByText(entry);
        }

        public override void toggleHighlight(GuiSession session) {}

        // This method cannot be implemented for a GridView Toolbar Button Menu ComboBox
        public override string getText(GuiSession session)
        {
            return "";
        }
    }

    public sealed class SAPToolbarMenu: ComboBox, ILabeled 
    {
        string id;
        string text;
        string toolbarId;
        string tooltip;

        public SAPToolbarMenu(GuiToolbarControl toolbar, int position) 
        {
            this.toolbarId = toolbar.Id;
            this.id = toolbar.GetButtonId(position);
            this.text = toolbar.GetButtonText(position);
            this.tooltip = toolbar.GetButtonTooltip(position);
        }

        public override bool contains(string entry)
        {
            return true;
        }

        public bool isHLabeled(string label) {
            return text == label;
        }

        public bool isVLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) 
        {
            return this.tooltip == tooltip;
        }

        public override void select(string entry, GuiSession session)
        {
            var toolbar = (GuiToolbarControl)session.FindById(toolbarId);
            toolbar.PressContextButton(id);
            toolbar.SelectContextMenuItemByText(entry);
        }

        public override void toggleHighlight(GuiSession session) {
            focused = !focused;
            var toolbar = (GuiToolbarControl)session.FindById(toolbarId);
            toolbar.Visualize(focused);
        }

        // This method cannot be implemented for a GuiShell of subtype Toolbar
        public override string getText(GuiSession session)
        {
            return "";
        }
    }
}
