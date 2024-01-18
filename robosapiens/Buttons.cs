using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens {
    public abstract class Button: IHighlightable {
        protected bool focused;
        public abstract void push(GuiSession session);
        public abstract void toggleHighlight(GuiSession session);
    }

    public class SAPButton: Button, ILabeled {
        protected string defaultTooltip;
        public string id;
        public Position position {get;}
        public string text;
        protected string tooltip;

        public SAPButton(GuiButton button) {
            this.defaultTooltip = button.DefaultTooltip;
            this.focused = false;
            this.id = button.Id;
            this.position = new Position(height: button.Height, 
                                left: button.ScreenLeft,
                                top: button.ScreenTop, 
                                width: button.Width
                            );
            this.text = button.Text.Trim();
            this.tooltip = button.Tooltip;
        }

        public bool isLabeled(string label) {
            return this.text == label;
        }

        public bool hasTooltip(string tooltip) {
            return this.tooltip.StartsWith(tooltip);
        }

        public override void push(GuiSession session) {
            var guiButton = (GuiButton)session.FindById(id);
            guiButton.Press();
        }

        public override void toggleHighlight(GuiSession session) {
            focused = !focused;
            var guiButton = (GuiButton)session.FindById(id);
            guiButton.Visualize(focused);
        }
    }

    public sealed class SAPTableButton: SAPButton, ILocatableCell {
        string column;
        int rowIndex;
        SAPTable table;

        public SAPTableButton(string column, int rowIndex, GuiButton button, SAPTable table): base(button) {
            this.column = column;
            this.rowIndex = rowIndex;
            this.table = table;
        }

        public bool isLocated(CellLocator locator, TextCellStore rowLabels) 
        {
            return column == locator.column && locator switch {
                RowCellLocator rowLocator => rowIndex == rowLocator.rowIndex - 1,
                LabelCellLocator labelLocator => isLabeled(labelLocator.label) ||
                                                 inRowOfCell(rowLabels.getByContent(labelLocator.label)),
                _ => false
            };
        }

        private bool inRowOfCell(TextCell? cell) 
        {
            return cell switch {
                TextCell => rowIndex == cell.rowIndex,
                _ => false
            };
        }

        public override void push(GuiSession session) {
            table.selectRow(rowIndex, session);
            var tableButton = (GuiButton)session.FindById(id);
            tableButton.Press();
        }
    }

    public sealed class SAPGridViewButton: Button, ILocatableCell {
        string columnId;
        public HashSet<string> columnTitles;
        string gridViewId;
        // string buttonId; // must start with &
        int rowIndex;
        string tooltip;

        public SAPGridViewButton(string columnId, GuiGridView gridView, int rowIndex) {
            this.columnId = columnId;
            this.columnTitles = new HashSet<string>(){};
            this.gridViewId = gridView.Id;
            // this.buttonId = buttonId;
            this.rowIndex = rowIndex;
            this.tooltip = gridView.GetCellTooltip(rowIndex, columnId);

            // GetDisplayedColumnTitle might not be reliable
            GuiCollection columnTitles = (GuiCollection)gridView.GetColumnTitles(columnId);
            for (int i = 0; i < columnTitles.Length; i++) 
            {
                this.columnTitles.Add((string)columnTitles.ElementAt(i));
            }
        }

        public bool isLabeled(string label) {
            return label == tooltip;
        }

        public bool isLocated(CellLocator locator, TextCellStore rowLabels) 
        {
            return columnTitles.Contains(locator.column) && locator switch {
                RowCellLocator rowLocator => rowIndex == rowLocator.rowIndex - 1,
                LabelCellLocator labelLocator => isLabeled(labelLocator.label) || 
                                                 inRowOfCell(rowLabels.getByContent(labelLocator.label)),
                _ => false
            };
        }

        private bool inRowOfCell(TextCell? cell) 
        {
            return cell switch {
                TextCell => rowIndex == cell.rowIndex,
                _ => false
            };
        }

        public override void push(GuiSession session) {
            GuiGridView gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.PressButton(rowIndex, columnId);
        }

        // From the documentation for GuiVComponent
        // Some components such as GuiCtrlGridView support displaying the frame around inner objects, 
        // such as cells. The format of the innerObject string is the same as for the dumpState method.
        public override void toggleHighlight(GuiSession session) {}
    }

    public sealed class SAPGridViewToolbarButton: Button, ILabeled {
        string gridViewId;
        string id;
        string tooltip;

        public SAPGridViewToolbarButton(GuiGridView gridView, string id, string tooltip) {
            this.gridViewId = gridView.Id;
            this.id = id;
            this.tooltip = tooltip;
        }

        public bool isLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) {
            return this.tooltip == tooltip;
        }
    
        public override void push(GuiSession session) {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.PressToolbarButton(id);
        }

        public override void toggleHighlight(GuiSession session)
        {
        }
    }

    public sealed class SAPGridViewToolbarButtonMenu: Button, ILabeled 
    {
        string gridViewId;
        string id;
        string tooltip;

        public SAPGridViewToolbarButtonMenu(GuiGridView gridView, string id, string tooltip) 
        {
            this.gridViewId = gridView.Id;
            this.id = id;
            this.tooltip = tooltip;
        }

        public bool isLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) {
            return this.tooltip == tooltip;
        }
    
        public override void push(GuiSession session) 
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.PressToolbarContextButton(id);
        }

        public override void toggleHighlight(GuiSession session) {}
    }

    public sealed class SAPToolbarButton: Button, ILabeled {
        string toolbarId;
        string id;
        string tooltip;

        public SAPToolbarButton(GuiToolbarControl toolbar, string id, string tooltip) {
            this.toolbarId = toolbar.Id;
            this.id = id;
            this.tooltip = tooltip;
        }

        public bool isLabeled(string label) {
            return false;
        }

        public bool hasTooltip(string tooltip) {
            return this.tooltip == tooltip;
        }
    
        public override void push(GuiSession session) {
            var toolbar = (GuiToolbarControl)session.FindById(toolbarId);
            toolbar.PressButton(id);
        }

        public override void toggleHighlight(GuiSession session)
        {
        }
    }

    public sealed class SAPTreeButton: Button, ILocatableCell {
        string columnName;
        string columnTitle;
        string nodeKey;
        string label;
        int rowNumber;
        string treeId;

        public SAPTreeButton(string columnName, string columnTitle, string label, string nodeKey, int rowNumber, string treeId) {
            this.columnName = columnName;
            this.columnTitle = columnTitle;
            this.nodeKey = nodeKey;
            this.label = label;
            this.rowNumber = rowNumber;
            this.treeId = treeId;
        }

        public bool isLabeled(string label) {
            return this.label == label;
        }

        public bool isLocated(CellLocator locator, TextCellStore rowLabels) 
        {
            return columnTitle == locator.column && locator switch {
                RowCellLocator rowLocator => rowLocator.rowIndex > 0 && rowNumber == rowLocator.rowIndex - 1,
                LabelCellLocator labelLocator => isLabeled(labelLocator.label),
                _ => false
            };
        }

        public override void push(GuiSession session) {
            var tree = (GuiTree)session.FindById(treeId);
            tree.PressButton(nodeKey, columnName);
        }

        public override void toggleHighlight(GuiSession session)
        {
        }
    }

    public sealed class SAPTreeLink: Button, ILocatableCell {
        string columnName;
        string columnTitle;
        string nodeKey;
        string text;
        string tooltip;
        string treeId;

        public SAPTreeLink(string columnName, string columnTitle, string text, string tooltip, string nodeKey, string treeId) {
            this.columnName = columnName;
            this.columnTitle = columnTitle;
            this.nodeKey = nodeKey;
            this.text = text;
            this.tooltip = tooltip;
            this.treeId = treeId;
        }

        public bool isLabeled(string label) {
            return this.tooltip == label || this.text == label;
        }

        public bool isLocated(CellLocator locator, TextCellStore rowLabels) 
        {
            return columnTitle == locator.column && locator switch {
                LabelCellLocator labelLocator => isLabeled(labelLocator.label),
                _ => false
            };
        }

        public override void push(GuiSession session) {
            var tree = (GuiTree)session.FindById(treeId);
            tree.ClickLink(nodeKey, columnName);
        }

        public override void toggleHighlight(GuiSession session)
        {
        }
    }
}
