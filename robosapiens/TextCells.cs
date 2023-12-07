using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens 
{
    public abstract class Cell: ITextElement 
    {
        public HashSet<string> columnTitles;
        public Position position;
        public int rowIndex;
        public string text;

        public Cell(int rowIndex, string column, string text) 
        {
            this.columnTitles = new HashSet<string>(){ column };
            this.rowIndex = rowIndex;
            this.text = text;
        }

        public bool contains(string content) 
        {
            return text == content || text.StartsWith(content);
        }

        public string getText() 
        {
            return text;
        }

        public bool isLocated(FilledCellLocator locator) 
        {
            return columnTitles.Contains(locator.column) && 
                locator.content switch {
                    string content => text == content,
                    _ => rowIndex == locator.rowIndex - 1
                };
        }

        public bool isLocated(EmptyCellLocator locator, LabelCellStore rowLabels) 
        {
            return columnTitles.Contains(locator.column) && 
                locator.rowLabel switch {
                    string rowLabel => inRowOfCell(rowLabels.getByContent(rowLabel)),
                    _ => rowIndex == locator.rowIndex - 1
                };
        }

        public abstract void select(GuiSession session);

        public abstract void toggleHighlight(GuiSession session);

        protected bool inRowOfCell(Cell? cell) 
        {
            return cell switch {
                Cell => rowIndex == cell.rowIndex,
                _ => false
            };
        }
    }

    public class SAPGridViewCell: Cell, IDoubleClickable, IFilledCell
    {
        public string columnId;
        protected string gridViewId;

        public SAPGridViewCell(string columnId, GuiGridView gridView, int rowIndex): 
            base(rowIndex, column: gridView.GetDisplayedColumnTitle(columnId), text: gridView.GetCellValue(rowIndex, columnId)) 
        {
            // GetDisplayedColumnTitle might not be reliable
            GuiCollection columnTitles = (GuiCollection)gridView.GetColumnTitles(columnId);
            for (int i = 0; i < columnTitles.Length; i++) 
            {
                this.columnTitles.Add((string)columnTitles.ElementAt(i));
            }

            this.columnId = columnId;
            gridViewId = gridView.Id;
            position = new Position(
                height: gridView.GetCellHeight(rowIndex, columnId), 
                left: gridView.GetCellLeft(rowIndex, columnId),
                top: gridView.GetCellTop(rowIndex, columnId), 
                width: gridView.GetCellWidth(rowIndex, columnId)
            );
        }

        public void doubleClick(GuiSession session) 
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.SetCurrentCell(rowIndex, columnId);
            gridView.DoubleClickCurrentCell();
        }

        public override void select(GuiSession session) 
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.SetCurrentCell(rowIndex, columnId);
            gridView.SelectedRows = rowIndex.ToString();
        }

        // From the documentation for GuiVComponent
        // Some components such as GuiCtrlGridView support displaying the frame around inner objects, 
        // such as cells. The format of the innerObject string is the same as for the dumpState method.
        public override void toggleHighlight(GuiSession session) {}
    }

    public sealed class EmptyGridViewCell: SAPGridViewCell, IEditableCell 
    {
        public EmptyGridViewCell(string columnId, GuiGridView gridView, int rowIndex): 
            base(columnId, gridView, rowIndex) {}

        public RobotResult.NotChangeable? insert(string content, GuiSession session) 
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);

            if (gridView.GetCellChangeable(rowIndex, columnId)) 
            {
                gridView.ModifyCell(rowIndex, columnId, content);
                text = content;
                return null;
            }
            else return new RobotResult.NotChangeable();
        }
    }

    public class SAPTableCell: Cell, IDoubleClickable, IFilledCell 
    {
        bool focused;
        public string id;
        public SAPTable table;

        public SAPTableCell(string column, int rowIndex, GuiTextField textField, SAPTable table):
            base(rowIndex, column, textField.Text) 
        {
            id = textField.Id;
            position = new Position(
                height: textField.Height, 
                left: textField.ScreenLeft,
                top: textField.ScreenTop, 
                width: textField.Width
            );
            this.table = table;
        }

        public void doubleClick(GuiSession session)
        {
            select(session);
            session.ActiveWindow.SendVKey((int)VKeys.getKeyCombination("F2")!);
        }

        public override void select(GuiSession session) 
        {
            var guiTextField = (GuiTextField)session.FindById(id);
            guiTextField.SetFocus();
        }

        public override void toggleHighlight(GuiSession session)
        {
            focused = !focused;
            var guiTextField = (GuiTextField)session.FindById(id);
            guiTextField.Visualize(focused);
        }
    }

    public sealed class EditableTableCell: SAPTableCell, IEditableCell 
    {
        public EditableTableCell(string column, int rowIndex, GuiTextField textField, SAPTable table):
            base(column, rowIndex, textField, table) {}

        public RobotResult.NotChangeable? insert(string content, GuiSession session) 
        {
            table.makeSureCellIsVisible(rowIndex, session);

            var textField = (GuiTextField)session.FindById(id);

            if (textField.Changeable) {
                textField.Text = content;
                text = content;
                return null;
            }
            else return new RobotResult.NotChangeable();
        }
    }

    public class SAPTreeCell: Cell, IDoubleClickable, IFilledCell 
    {
        string nodeKey;
        string treeId;

        public SAPTreeCell(string columnName, string columnTitle, int rowIndex, string content, string nodeKey, GuiTree tree):
            base(rowIndex, columnTitle, content)
        {
            this.nodeKey = nodeKey;
            position = new Position(
                height: tree.GetItemHeight(nodeKey, columnName), 
                left: tree.GetItemLeft(nodeKey, columnName),
                top: tree.GetItemTop(nodeKey, columnName), 
                width: tree.GetItemWidth(nodeKey, columnName)
            );
            treeId = tree.Id;
        }

        public void doubleClick(GuiSession session) 
        {
            var tree = (GuiTree)session.FindById(treeId);
            tree.DoubleClickNode(nodeKey);
        }

        public override void select(GuiSession session) 
        {
            var tree = (GuiTree)session.FindById(treeId);
            tree.SelectNode(nodeKey);
        }

        public override void toggleHighlight(GuiSession session) {}
    }
}
