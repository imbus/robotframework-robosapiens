using sapfewse;

namespace RoboSAPiens {
    public sealed class SAPBox: ILabeled {
        string title;
        Position position;

        public SAPBox(GuiBox box) {
            this.position = new Position(height: box.Height, 
                                left: box.ScreenLeft,
                                top: box.ScreenTop, 
                                width: box.Width);
            this.title = box.Text;
        }

        public bool contains(Position other) {
            return other.left > position.left && 
                   other.right < position.right &&
                   other.top > position.top &&
                   other.bottom < position.bottom;
        }

        public bool isHLabeled(string label) {
            return label.Equals(title);
        }

        public bool isVLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) {
            return false;
        }
    }
}
