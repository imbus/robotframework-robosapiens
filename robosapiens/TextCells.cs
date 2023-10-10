using sapfewse;

namespace RoboSAPiens {
    public abstract class Cell: ITextElement {
        public string column;
        public Position position;
        public int rowIndex;
        public string text;

        public Cell(int rowIndex, string column, string text) {
            this.column = column;
            this.rowIndex = rowIndex;
            this.text = text;
        }

        public bool contains(string content) {
            return text == content || text.StartsWith(content);
        }

        public string getText() {
            return text;
        }

        public abstract void select(GuiSession session);

        protected bool inRowOfCell(Cell? cell) {
            return cell switch {
                Cell => rowIndex == cell.rowIndex,
                _ => false
            };
        }
    }

    public class SAPGridViewCell: Cell, IDoubleClickable, IFilledCell {
        public string columnId;
        protected string gridViewId;

        public SAPGridViewCell(string columnId, GuiGridView gridView, int rowIndex): 
            base(rowIndex, column: gridView.GetDisplayedColumnTitle(columnId), text: gridView.GetCellValue(rowIndex, columnId)) 
        {
            this.columnId = columnId;
            gridViewId = gridView.Id;
            position = new Position(height: gridView.GetCellHeight(rowIndex, columnId), 
                                         left: gridView.GetCellLeft(rowIndex, columnId),
                                         top: gridView.GetCellTop(rowIndex, columnId), 
                                         width: gridView.GetCellWidth(rowIndex, columnId)
                                        );
        }

        public void doubleClick(GuiSession session) {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.SetCurrentCell(rowIndex, columnId);
            gridView.DoubleClickCurrentCell();
        }

        public bool isLocated(FilledCellLocator locator) {
            return column == locator.column && 
                locator.content switch {
                    string content => text == content,
                    _ => rowIndex == locator.rowIndex - 1
                };
        }

        public override void select(GuiSession session) {
            var gridView = (GuiGridView)session.FindById(gridViewId);
            gridView.SetCurrentCell(rowIndex, columnId);
            gridView.SelectedRows = rowIndex.ToString();
        }
    }

    public sealed class EmptyGridViewCell: SAPGridViewCell, IEditableCell {
        int? maxLength;

        public EmptyGridViewCell(string columnId, GuiGridView gridView, int rowIndex): 
            base(columnId, gridView, rowIndex) 
        {
            try {
                // Does not work with SAP Logon 7.60 PL 0. It works with 7.60 PL 2.
                // https://answers.sap.com/questions/13286657/sap-grid-view-control-getcellmaxlength-throw-excep.html
                maxLength = gridView.GetCellMaxLength(rowIndex, columnId);
            }
            catch {
                maxLength = null;
            }
        }

        public int? getMaxLength() {
            return maxLength;
        }

        public RobotResult.NotChangeable? insert(string content, GuiSession session) {
            var gridView = (GuiGridView)session.FindById(gridViewId);

            if (gridView.GetCellChangeable(rowIndex, columnId)) {
                gridView.ModifyCell(rowIndex, columnId, content);
                text = content;
                return null;
            }
            else return new RobotResult.NotChangeable();
        }

        public bool isLocated(EmptyCellLocator locator, LabelCellStore rowLabels) {
            return column == locator.column && 
                locator.rowLabel switch {
                    string rowLabel => inRowOfCell(rowLabels.getByContent(rowLabel)),
                    _ => rowIndex == locator.rowIndex - 1
                };
        }

        // From the documentation for GuiVComponent
        // Some components such as GuiCtrlGridView support displaying the frame around inner objects, 
        // such as cells. The format of the innerObject string is the same as for the dumpState method.
        public void toggleHighlight(GuiSession session){
        }
    }

    public class SAPTableCell: Cell, IDoubleClickable, IFilledCell {
        public string id;
        public SAPTable table;

        public SAPTableCell(string column, int rowIndex, GuiTextField textField, SAPTable table):
            base(rowIndex, column, textField.Text) 
        {
            id = textField.Id;
            position = new Position(height: textField.Height, 
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

        public bool isLocated(FilledCellLocator locator) {
            return column == locator.column && 
                locator.content switch {
                    string content => text == content,
                    _ => rowIndex == locator.rowIndex - 1
                };
        }

        public override void select(GuiSession session) {
            var guiTextField = (GuiTextField)session.FindById(id);
            guiTextField.SetFocus();
        }
    }

    public sealed class EditableTableCell: SAPTableCell, IEditableCell {
        int? maxLength;
        bool focused;

        public EditableTableCell(string column, int rowIndex, GuiTextField textField, SAPTable table):
            base(column, rowIndex, textField, table) 
        {
            maxLength = textField.MaxLength;
        }

        public int? getMaxLength() {
            return maxLength;
        }

        public RobotResult.NotChangeable? insert(string content, GuiSession session) {
            table.makeSureCellIsVisible(rowIndex, session);

            var textField = (GuiTextField)session.FindById(id);

            if (textField.Changeable) {
                textField.Text = content;
                text = content;
                return null;
            }
            else return new RobotResult.NotChangeable();
        }

        public bool isLocated(EmptyCellLocator locator, LabelCellStore rowLabels) {
            return column == locator.column && 
                locator.rowLabel switch {
                    string rowLabel => inRowOfCell(rowLabels.getByContent(rowLabel)),
                    _ => rowIndex == locator.rowIndex - 1
                };
        }

        public void toggleHighlight(GuiSession session){
            focused = !focused;
            var guiTextField = (GuiTextField)session.FindById(id);
            guiTextField.Visualize(focused);
        }
    }

    public class SAPTreeCell: Cell, IDoubleClickable, IFilledCell {
        string columnName;
        string nodeKey;
        string treeId;

        public SAPTreeCell(string columnName, string columnTitle, int rowIndex, string content, string nodeKey, GuiTree tree):
            base(rowIndex, columnTitle, content)
        {
            this.columnName = columnName;
            this.nodeKey = nodeKey;
            position = new Position(height: tree.GetItemHeight(nodeKey, columnName), 
                                         left: tree.GetItemLeft(nodeKey, columnName),
                                         top: tree.GetItemTop(nodeKey, columnName), 
                                         width: tree.GetItemWidth(nodeKey, columnName)
                                        );
            treeId = tree.Id;
        }

        public bool isLocated(FilledCellLocator locator) {
            return column == locator.column && 
                locator.content switch {
                    string content => text == content,
                    _ => rowIndex == locator.rowIndex - 1
                };
        }

        public void doubleClick(GuiSession session) {
            var tree = (GuiTree)session.FindById(treeId);
            tree.DoubleClickNode(nodeKey);
        }

        public override void select(GuiSession session) {
            var tree = (GuiTree)session.FindById(treeId);
            tree.SelectItem(nodeKey, columnName);
        }
    }
}
