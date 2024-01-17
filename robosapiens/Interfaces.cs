using sapfewse;

namespace RoboSAPiens {
    public interface IDoubleClickable {
        public void doubleClick(GuiSession session);
    }

    public interface IHighlightable {
        public void toggleHighlight(GuiSession session);
    }

    public interface IIndexLocator: ILocator{}

    public interface ILabelsLocator: ILocator{}

    public interface ILabeled {
        public bool isLabeled(string label);
    }

    public interface ILocatable {
        public bool isLocated(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels);
        public Position getPosition();
    }

    public interface ILocatableCell {
        public bool isLocated(CellLocator locator, TextCellStore labelCells);
    }

    public interface ILocator {}

    public interface ISelectable {
        public void select(GuiSession session);
    }

    public interface ISession {}

    public interface ITextElement: ISelectable, IHighlightable {
        public bool contains(string text);
        public string getText();
    }

    public interface ILogger {
        public void error(params string[] messages);
        public void info(params string[] messages);
    }
}
