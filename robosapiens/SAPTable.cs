using sapfewse;

namespace RoboSAPiens {
    public class SAPTable: ITable {
        public string id {get;}
        int visibleRowCount;
        public int totalRows;
        int rowsInStore;

        public SAPTable(GuiTableControl table) {
            this.id = table.Id;
            
            // https://answers.sap.com/questions/11100660/how-to-get-a-correct-row-count-in-sap-table.html
            this.totalRows = table.VerticalScrollbar.Maximum + 1;
            this.visibleRowCount = table.VisibleRowCount;
            this.rowsInStore = 0;
        }

        public int getNumRows() {
            return totalRows;
        }

        public void updateRowsInStore(int rowsAdded) {
            rowsInStore += rowsAdded;
        }

        public int getRowsInStore() {
            return rowsInStore;
        }

        public bool rowsAreMissing() {
            return rowsInStore < totalRows;
        }

        public void makeSureCellIsVisible(int rowIndex, GuiSession session) {
            if (rowIndex + 1 > visibleRowCount) {
                scrollOnePage(session);
            }
        }

        public void selectRow(int rowIndex, GuiSession session) {
            var table = (GuiTableControl)session.FindById(id);
            var rows = table.Rows;
            var row = (GuiTableRow)rows.ElementAt(rowIndex);
            row.Selected = true;
        }

        public bool scrollOnePage(GuiSession session) {
            var table = (GuiTableControl)session.FindById(id);

            // When a GuiTableControl is scrolled a number of times 
            // that is not a multiple of 3 the whole table becomes write-protected
            if (table.VerticalScrollbar.Position + 3 < table.VerticalScrollbar.Maximum) 
            {
                table.VerticalScrollbar.Position += 3;
                return true;
            }

            return false;
        }
    }
}
