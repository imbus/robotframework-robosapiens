using sapfewse;

namespace SAPiens {
    public sealed class SAPTab: ILabeled, IHighlightable {
        string tooltip;
        string id;
        string label;
        private bool focused;

        public SAPTab(GuiTab tab) {
            this.tooltip = tab.Tooltip;
            this.id = tab.Id;
            this.label = tab.Text;
        }

        public bool isLabeled(string label) {
            return this.label == label ||
                   this.tooltip == label;
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
