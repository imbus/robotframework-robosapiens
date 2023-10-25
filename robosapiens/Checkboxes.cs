using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens {
    public abstract class CheckBox: IHighlightable {
        protected bool focused;
        public abstract void select(GuiSession session);
        public abstract void deselect(GuiSession session);
        public abstract void toggleHighlight(GuiSession session);
    }

    public class SAPCheckBox: CheckBox, ILabeled, ILocatable, ISelectable {
        string defaultTooltip;
        string id;
        Position position;
        string text;

        public SAPCheckBox(GuiCheckBox checkBox) {
            this.defaultTooltip = checkBox.DefaultTooltip;
            this.id = checkBox.Id;
            this.position = new Position(height: checkBox.Height, 
                                left: checkBox.ScreenLeft,
                                top: checkBox.ScreenTop, 
                                width: checkBox.Width);
            this.text = checkBox.Text;
        }

        public Position getPosition() {
            return position;
        }

        public bool isHorizontalAlignedWithLabel(SAPLabel? label) {
            return label switch {
                SAPLabel => label.position.horizontalAlignedWith(position),
                _ => false
            };
        }

        public bool isHorizontalAlignedWithTextField(SAPTextField? textField) {
            return textField switch {
                SAPTextField => textField.position.horizontalAlignedWith(position),
                _ => false
            };
        }

        public bool isVerticalAlignedWithLabel(SAPLabel? label) {
            return label switch {
                SAPLabel => label.position.verticalAlignedWith(position),
                _ => false
            };
        }

        public bool isLabeled(string label) {
            return text == label || 
                   defaultTooltip == label;
        }

        public virtual bool isLocated(ILocator locator, LabelStore labels, ReadOnlyTextFieldStore textFieldLabels) {
            return locator switch {
                HLabelVLabel(var hLabel, var vLabel) =>
                isHorizontalAlignedWithLabel(labels.getByName(hLabel)) &&
                isVerticalAlignedWithLabel(labels.getByName(vLabel)),
                _ => false
            };
        }

        public override void select(GuiSession session) {
            var guiCheckBox = (GuiCheckBox)session.FindById(id);
            guiCheckBox.Selected = true;
        }

        public override void deselect(GuiSession session) {
            var guiCheckBox = (GuiCheckBox)session.FindById(id);
            guiCheckBox.Selected = false;
        }

        public override void toggleHighlight(GuiSession session) {
            focused = !focused;
            var guiCheckBox = (GuiCheckBox)session.FindById(id);
            guiCheckBox.Visualize(focused);
        }
    }

    public sealed class SAPTableCheckBox: SAPCheckBox, IFilledCell {
        string column;
        int rowIndex;
        SAPTable table;

        public SAPTableCheckBox(string column, int rowIndex, GuiCheckBox checkBox, SAPTable table): base(checkBox) {
            this.column = column;
            this.table = table;
            this.rowIndex = rowIndex;
        }

        public bool isLocated(FilledCellLocator locator) {
            return column == locator.column && 
                   rowIndex == locator.rowIndex - 1;
        }
    }

    public sealed class SAPTreeCheckBox: CheckBox, IFilledCell {
        string columnName;
        string columnTitle;
        string nodeKey;
        int rowNumber;
        string treeId;

        public SAPTreeCheckBox(string columnName, string columnTitle, string nodeKey, int rowNumber, string treeId) {
            this.columnName = columnName;
            this.columnTitle = columnTitle;
            this.nodeKey = nodeKey;
            this.rowNumber = rowNumber;
            this.treeId = treeId;
        }

        public bool isLocated(FilledCellLocator locator) {
            if (locator.rowIndex > 0) {
                return rowNumber == locator.rowIndex - 1 && columnTitle == locator.column;
            }
            return false;
        }

        public override void select(GuiSession session) {
            var tree = (GuiTree)session.FindById(treeId);
            tree.SetCheckBoxState(nodeKey, columnName, 1);
        }

        public override void deselect(GuiSession session) {
            var tree = (GuiTree)session.FindById(treeId);
            tree.SetCheckBoxState(nodeKey, columnName, 0);
        }

        public override void toggleHighlight(GuiSession session) {
            focused = !focused;
            var tree = (GuiTree)session.FindById(treeId);
            tree.Visualize(focused);
        }
    }

    public sealed class SAPGridViewCheckBox: CheckBox, IFilledCell, ISelectable {
        string columnId;
        public HashSet<string> columnTitles;
        string gridViewId;
        int rowIndex;

        public SAPGridViewCheckBox(string columnId, GuiGridView gridView, int rowIndex) {
            this.columnId = columnId;
            this.columnTitles = new HashSet<string>(){};
            this.gridViewId = gridView.Id;
            this.rowIndex = rowIndex;

            // GetDisplayedColumnTitle might not be reliable
            GuiCollection columnTitles = (GuiCollection)gridView.GetColumnTitles(columnId);
            for (int i = 0; i < columnTitles.Length; i++) 
            {
                this.columnTitles.Add((string)columnTitles.ElementAt(i));
            }
        }

        public bool isLocated(FilledCellLocator locator) {
            return columnTitles.Contains(locator.column) && 
                   rowIndex == locator.rowIndex - 1;
        }

        public override void select(GuiSession session) {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.ModifyCheckBox(rowIndex, columnId, true);        
        }

        public override void deselect(GuiSession session) {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.ModifyCheckBox(rowIndex, columnId, false);        
        }

        // From the documentation for GuiVComponent
        // Some components such as GuiCtrlGridView support displaying the frame around inner objects, 
        // such as cells. The format of the innerObject string is the same as for the dumpState method.
        public override void toggleHighlight(GuiSession session) {}
    }
}
