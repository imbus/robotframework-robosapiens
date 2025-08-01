using System.Collections.Generic;
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
        public bool isHLabeled(string label);
        public bool isVLabeled(string label);
        public bool hasTooltip(string tooltip);
    }

    public interface ILocatable {
        public bool isLocated(ILocator locator, LabelStore labels, TextFieldRepository textFieldLabels);
        public Position getPosition();
    }

    public interface ILocator {}

    public interface ISelectable: IHighlightable {
        public void select(GuiSession session);
    }

    public interface ISession {
        public SessionInfo? getSessionInfo();
    }

    public interface ITextElement: ISelectable {
        public bool contains(string text);
        public string getText(GuiSession session);
    }

    public interface ILogger {
        public void error(params string[] messages);
        public void info(params string[] messages);
    }

    public interface ITable
    {
        public void classifyCells(GuiSession session);
        public Cell? findCell(ILocator locator, GuiSession session);
        public int getNumRows(GuiSession session);
        public bool hasColumn(string column);
        public bool rowIsAbove(GuiSession session, int rowIndex);
        public bool rowIsBelow(GuiSession session, int rowIndex);
        public bool scrollOnePage(GuiSession session);
        public void selectColumn(string column, GuiSession session);
        public void selectRow(int rowNumber, GuiSession session);
        public void selectRows(List<int> rowIndices, GuiSession session);
    }
}
