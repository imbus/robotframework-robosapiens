using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens {
    public abstract class ComboBox {
        public abstract bool contains(string entry);
        public abstract void select(string entry, GuiSession session);
    }

    public sealed class SAPComboBox: ComboBox, ILabeled {
        List<string> entries;
        string id;
        string label;

        public SAPComboBox(GuiComboBox comboBox) {
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
            return label == this.label;
        }

        public override void select(string entry, GuiSession session) {
            var guiComboBox = (GuiComboBox)session.FindById(id);
            guiComboBox.Value = entry;
        }
    }

    // public class GridViewValueList: ComboBox, ILocated {
    // }
}
