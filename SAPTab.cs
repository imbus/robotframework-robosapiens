using SAPFEWSELib;

namespace RoboSAPiens {
    public sealed class SAPTab: ILabeled {
        string id;
        string label;

        public SAPTab(GuiTab tab) {
            this.id = tab.Id;
            this.label = tab.Text;
        }

        public bool isLabeled(string label) {
            return this.label.Equals(label);
        }

        public void select(GuiSession session) {
            var tab = (GuiTab)session.FindById(id);
            tab.Select();
        }
    }
}
