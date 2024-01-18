using sapfewse;

namespace RoboSAPiens {
    public sealed class SAPTab: ILabeled, IHighlightable {
        string tooltip;
        string id;
        string label;
        private bool focused;

        public SAPTab(GuiTab tab) {
            this.tooltip = tab.Tooltip;
            this.id = tab.Id;
            this.label = tab.Text.Trim();
        }

        public bool isLabeled(string label) {
            return this.label == label;
        }

        public bool hasTooltip(string tooltip) {
            return this.tooltip == tooltip;
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
