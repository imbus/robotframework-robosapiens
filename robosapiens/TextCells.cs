using System.Collections.Generic;
using sapfewse;

namespace RoboSAPiens 
{
    public abstract class TextCell: ITextElement, ILocatableCell, IDoubleClickable
    {
        protected bool focused;
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

        public abstract void doubleClick(GuiSession session);

        public bool contains(string content) 
        {
            return text == content || text.StartsWith(content);
        }

        public abstract string getText(GuiSession session); 

        public abstract void insert(string content, GuiSession session);

        public abstract bool isChangeable(GuiSession session);

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

    public class SAPGridViewCell: TextCell
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
            this.columnTitles.Add(gridView.GetColumnTooltip(columnId).Trim());

            this.columnId = columnId;
            gridViewId = gridView.Id;
            position = new Position(
                height: gridView.GetCellHeight(rowIndex, columnId), 
                left: gridView.GetCellLeft(rowIndex, columnId),
                top: gridView.GetCellTop(rowIndex, columnId), 
                width: gridView.GetCellWidth(rowIndex, columnId)
            );
        }

        public override void doubleClick(GuiSession session) 
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

        public override void insert(string content, GuiSession session) 
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.ModifyCell(rowIndex, columnId, content);
            text = content;
        }

        public override bool isChangeable(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            return gridView.GetCellChangeable(rowIndex, columnId);
        }

        public override void select(GuiSession session) 
        {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.SetCurrentCell(rowIndex, columnId);
            gridView.SelectedRows = rowIndex.ToString();
        }

        // The innerObject parameter of the Visualize method of GuiGridView
        // can only take the values "Toolbar" and "Cell(row,column)".
        // References:
        // https://community.sap.com/t5/technology-q-a/get-innerobject-for-visualizing/qaq-p/12150835
        // https://www.synactive.com/download/sap%20gui%20scripting/sap%20gui%20scripting%20api.pdf
        public override void toggleHighlight(GuiSession session) {
            focused = !focused;
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.Visualize(focused, $"Cell({rowIndex},{columnId})");
        }
    }


    public class SAPTableCell: TextCell
    {
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

        public override void doubleClick(GuiSession session)
        {
            select(session);
            session.ActiveWindow.SendVKey((int)VKeys.getKeyCombination("F2")!);
        }

        public override string getText(GuiSession session)
        {
            var textField = (GuiTextField)session.FindById(id);
            return textField.Text;
        }

        public override void insert(string content, GuiSession session) 
        {
            table.makeSureCellIsVisible(rowIndex, session);

            var textField = (GuiTextField)session.FindById(id);
            textField.Text = content;
            text = content;
        }

        public override bool isChangeable(GuiSession session)
        {
            var textField = (GuiTextField)session.FindById(id);
            return textField.Changeable;
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

    public class SAPTreeCell: TextCell
    {
        string columnName;
        string nodeKey;
        string treeId;

        public SAPTreeCell(string columnName, string columnTitle, int rowIndex, string content, string nodeKey, GuiTree tree):
            base(rowIndex, columnTitle, content)
        {
            this.columnName = columnName;
            this.nodeKey = nodeKey;
            position = new Position(
                height: tree.GetItemHeight(nodeKey, columnName), 
                left: tree.GetItemLeft(nodeKey, columnName),
                top: tree.GetItemTop(nodeKey, columnName), 
                width: tree.GetItemWidth(nodeKey, columnName)
            );
            treeId = tree.Id;
        }

        public override void doubleClick(GuiSession session) 
        {
            var tree = (GuiTree)session.FindById(treeId);
            tree.DoubleClickNode(nodeKey);
        }

        public override string getText(GuiSession session)
        {
            var tree = (GuiTree)session.FindById(treeId);
            return tree.GetItemText(nodeKey, columnName);
        }

        public override void insert(string content, GuiSession session) 
        {
        }

        public override bool isChangeable(GuiSession session)
        {
            return false;
        }

        public override void select(GuiSession session) 
        {
            var tree = (GuiTree)session.FindById(treeId);
            tree.SelectNode(nodeKey);
        }

        public override void toggleHighlight(GuiSession session) {}
    }
}
