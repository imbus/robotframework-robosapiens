using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens {
    public abstract class ComboBox: IHighlightable {
        protected bool focused;
        public abstract bool contains(string entry);
        public abstract void select(string entry, GuiSession session);
        public abstract void toggleHighlight(GuiSession session);
    }

    public class SAPComboBox: ComboBox, ILabeled {
        string accTooltip;
        List<string> entries;
        string id;
        string label;

        public SAPComboBox(GuiComboBox comboBox) {
            accTooltip = comboBox.AccTooltip.Trim();
            entries = new List<string>();
            id = comboBox.Id;
            label = getLeftLabel(comboBox);
            getEntries(comboBox);
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

        public bool isLabeled(string label) {
            return this.label == label || 
                   accTooltip == label;
            }

        public override void select(string entry, GuiSession session) {
            var guiComboBox = (GuiComboBox)session.FindById(id);
            guiComboBox.Value = entry;
        }

        public override void toggleHighlight(GuiSession session) {
            focused = !focused;
            var guiComboBox = (GuiComboBox)session.FindById(id);
            guiComboBox.Visualize(focused);
        }
    }

    public sealed class SAPTableComboBox: SAPComboBox, IFilledCell {
        string column;
        int rowIndex;

        public SAPTableComboBox(string column, int rowIndex, GuiComboBox comboBox): base(comboBox) 
        {
            this.column = column;
            this.rowIndex = rowIndex;
        }

        public bool isLocated(FilledCellLocator locator)
        {
            return column == locator.column && 
                   rowIndex == locator.rowIndex - 1;
        }
    }

    public sealed class SAPGridViewToolbarButtonMenuComboBox: ComboBox, ILabeled 
    {
        string gridViewId;
        string tooltip;

        public SAPGridViewToolbarButtonMenuComboBox(GuiGridView gridView, string tooltip) 
        {
            this.gridViewId = gridView.Id;
            this.tooltip = tooltip;
        }

        public override bool contains(string entry)
        {
            return true;
        }

        public bool isLabeled(string label) 
        {
            return tooltip == label;
        }

        public override void select(string entry, GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.SelectContextMenuItemByText(entry);
        }

        public override void toggleHighlight(GuiSession session) {}
    }

    public sealed class SAPGridViewToolbarMenu: ComboBox, ILabeled 
    {
        string gridViewId;
        string tooltip;

        public SAPGridViewToolbarMenu(GuiGridView gridView, string tooltip) 
        {
            this.gridViewId = gridView.Id;
            this.tooltip = tooltip;
        }

        public override bool contains(string entry)
        {
            return true;
        }

        public bool isLabeled(string label) 
        {
            return tooltip == label;
        }

        public override void select(string entry, GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.SelectContextMenuItemByText(entry);
        }

        public override void toggleHighlight(GuiSession session) {}
    }


    // public class GridViewValueList: ComboBox, ILocated {}
    // According to SAP it is not possible to automate a GuiGridView cell of type ValueList
    // https://answers.sap.com/questions/13469233/sap-gui-scripting-selecting-valuelist-in-a-guigrid.html
}
