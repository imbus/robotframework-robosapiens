using sapfewse;

namespace RoboSAPiens {
    public sealed class SAPTab: ILabeled, IHighlightable {
        string id;
        string label;
        private bool focused;

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

        public void toggleHighlight(GuiSession session) {
            focused = !focused;
            var tab = (GuiTab)session.FindById(id);
            tab.Visualize(focused);
        }
    }
}
