using sapfewse;

namespace RoboSAPiens {
    public sealed class SAPTab: ILabeled, IHighlightable {
        string tooltip;
        string id;
        string label;
        private bool focused;
        Position position;

        public SAPTab(GuiTab tab) {
            this.id = tab.Id;
            this.label = tab.Text.Trim();
            this.position = new Position(
                height: tab.Height, 
                left: tab.ScreenLeft,
                top: tab.ScreenTop, 
                width: tab.Width
            );
            this.tooltip = tab.Tooltip;
        }

        
        public bool contains(Position other, string otherId, GuiSession session) {
            return other.top > position.top && containsId(otherId, session);
        }
        
        bool containsId(string otherId, GuiSession session) {
            try
            {
                var tab = (GuiTab)session.FindById(id);
                tab.FindById(otherId);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public bool isHLabeled(string label) {
            return this.label == label;
        }

        public bool isVLabeled(string label) {
            return false;
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
