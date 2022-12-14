using sapfewse;

namespace RoboSAPiens {
    public interface IDoubleClickable {
        public void doubleClick(GuiSession session);
    }

    public interface IEditableCell {
        public int? getMaxLength();
        public void insert(string text, GuiSession session);
        public bool isLocated(EmptyCellLocator locator, LabelCellStore rowLabels);
    }

    public interface IFilledCell {
        public bool isLocated(FilledCellLocator locator);
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

    public interface ILocator {}

    public interface ISelectable {
        public void select(GuiSession session);
    }

    public interface ISession {}

    public interface ITextElement: ISelectable {
        public bool contains(string text);
        public string getText();
    }
}
