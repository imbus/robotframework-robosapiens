using sapfewse;
using System;

namespace RoboSAPiens {
    public class SAPTable {
        public string id {get;}
        int visibleRowCount;
        int totalRows;
        int rowsInStore;

        public SAPTable(GuiTableControl table) {
            this.id = table.Id;
            
            // https://answers.sap.com/questions/11100660/how-to-get-a-correct-row-count-in-sap-table.html
            this.totalRows = table.VerticalScrollbar.Maximum + 1;
            this.visibleRowCount = table.VisibleRowCount;
            this.rowsInStore = 0;
        }

        public int getNumRows() {
            return Math.Min(visibleRowCount, totalRows - rowsInStore);
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
            var rows = (GuiCollection)table.Rows;
            var row = (GuiTableRow)rows.ElementAt(rowIndex);
            row.Selected = true;
        }

        public void scrollOnePage(GuiSession session) {
            var table = (GuiTableControl)session.FindById(id);
            // CAUTION: Changing the scrollbar position redraws the GUI components.
            // Therefore, all object references are lost.
            table.VerticalScrollbar.Position += table.VisibleRowCount;
        }

        public void scrollToTop(GuiSession session) {
            var table = (GuiTableControl)session.FindById(id);
            table.VerticalScrollbar.Position = table.VerticalScrollbar.Minimum;
        }
    }
}
