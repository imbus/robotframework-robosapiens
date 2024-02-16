using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens 
{
    public abstract class TextCell: ITextElement, ILocatableCell
    {
        public HashSet<string> columnTitles;
        public Position position;
        public int rowIndex;
        public string text;

        public TextCell(int rowIndex, string column, string text) 
        {
            this.columnTitles = new HashSet<string>(){ column };
            this.rowIndex = rowIndex;
            this.text = text;
        }

        public bool contains(string content) 
        {
            return text == content || text.StartsWith(content);
        }

        public abstract string getText(GuiSession session); 

        public abstract RobotResult.NotChangeable? insert(string content, GuiSession session);

        public bool isLocated(CellLocator locator, TextCellStore rowLabels) 
        {
            return columnTitles.Contains(locator.column) && locator switch {
                RowCellLocator rowLocator => rowIndex == rowLocator.rowIndex - 1,
                LabelCellLocator labelLocator => text == labelLocator.label ||
                                                 inRowOfCell(rowLabels.getByContent(labelLocator.label)),
                _ => false
            };
        }

        public abstract void select(GuiSession session);

        public abstract void toggleHighlight(GuiSession session);

        protected bool inRowOfCell(TextCell? cell) 
        {
            return cell switch {
                TextCell => rowIndex == cell.rowIndex,
                _ => false
            };
        }
    }

    public class SAPGridViewCell: TextCell, IDoubleClickable
    {
        public string columnId;
        protected string gridViewId;

        public SAPGridViewCell(string columnId, GuiGridView gridView, int rowIndex): 
            base(rowIndex, column: gridView.GetDisplayedColumnTitle(columnId), text: gridView.GetCellValue(rowIndex, columnId)) 
        {
            // GetDisplayedColumnTitle might not be reliable
            this.columnTitles.Add(gridView.GetColumnTooltip(columnId).Trim());
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

        public override string getText(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            return gridView.GetCellValue(rowIndex, columnId);
        }

            if (gridView.GetCellChangeable(rowIndex, columnId)) 
            {
                gridView.ModifyCell(rowIndex, columnId, content);
                text = content;
                return null;
            }
            else return new RobotResult.NotChangeable();
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


    public class SAPTableCell: TextCell, IDoubleClickable
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

        public override string getText(GuiSession session)
        {
            var textField = (GuiTextField)session.FindById(id);
            return textField.Text;
        }
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

    public class SAPTreeCell: TextCell, IDoubleClickable
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

        public override string getText(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(treeId);
            return tree.GetNodeTextByKey(nodeKey);
        }
        {
            return new RobotResult.NotChangeable();
        }

        public override void select(GuiSession session) 
        {
            var tree = (GuiTree)session.FindById(treeId);
            tree.SelectNode(nodeKey);
        }

        public override void toggleHighlight(GuiSession session) {}
    }
}
