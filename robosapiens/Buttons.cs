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
            return this.text == label || this.tooltip.StartsWith(label);
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

    public sealed class SAPTableButton: SAPButton, IFilledCell {
        string column;
        int rowIndex;
        SAPTable table;

        public SAPTableButton(string column, int rowIndex, GuiButton button, SAPTable table): base(button) {
            this.column = column;
            this.rowIndex = rowIndex;
            this.table = table;
        }

        public bool isLocated(FilledCellLocator locator) {
            return locator.content switch {
                string content => isLabeled(content) && column == locator.column,
                _ => rowIndex == locator.rowIndex - 1 && column == locator.column
            };
        }

        public override void push(GuiSession session) {
            table.selectRow(rowIndex, session);
            var tableButton = (GuiButton)session.FindById(id);
            tableButton.Press();
        }
    }

    public sealed class SAPGridViewButton: Button, IFilledCell {
        string gridViewId;
        // string buttonId; // must start with &
        int rowIndex;
        string tooltip;

        public SAPGridViewButton(string columnId, GuiGridView gridView, int rowIndex) {
            this.gridViewId = gridView.Id;
            // this.buttonId = buttonId;
            this.rowIndex = rowIndex;
            this.tooltip = gridView.GetCellTooltip(rowIndex, columnId);
        }

        public bool isLabeled(string label) {
            return label == tooltip;
        }

        public bool isLocated(FilledCellLocator locator) {
            return locator.content switch {
                string content => isLabeled(content),
                _ => rowIndex == locator.rowIndex
            };
        }

        public override void push(GuiSession session) {
        }

        public override void toggleHighlight(GuiSession session)
        {
        }

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
            return tooltip == label;
        }
    
        public override void push(GuiSession session) {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.PressToolbarButton(id);
        }

        public override void toggleHighlight(GuiSession session)
        {
        }
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
            return tooltip == label;
        }
    
        public override void push(GuiSession session) {
            var toolbar = (GuiToolbarControl)session.FindById(toolbarId);
            toolbar.PressButton(id);
        }

        public override void toggleHighlight(GuiSession session)
        {
        }
    }

    public sealed class SAPTreeButton: Button, IFilledCell {
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

        public bool isLocated(FilledCellLocator locator) {
            if (locator.rowIndex > 0) {
                return rowNumber == locator.rowIndex - 1 && columnTitle == locator.column;
            }

            if (locator.content != null) {
                return columnTitle == locator.column && isLabeled(locator.content);
            }
            return false;
        }

        public override void push(GuiSession session) {
            var tree = (GuiTree)session.FindById(treeId);
            tree.PressButton(nodeKey, columnName);
        }

        public override void toggleHighlight(GuiSession session)
        {
        }
    }

    public sealed class SAPTreeLink: Button, IFilledCell {
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

        public bool isLocated(FilledCellLocator locator) {
            if (locator.content != null) {
                return columnTitle == locator.column && isLabeled(locator.content);
            }
            return false;
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
