using sapfewse;

namespace RoboSAPiens {
    public class SAPGridView {
        string id;
        int rowCount;

        public SAPGridView(GuiGridView guiGridView) {
            id = guiGridView.Id;
            rowCount = guiGridView.RowCount;
        }

        public int getNumRows()
        {
            return rowCount;
        }

        // https://answers.sap.com/questions/290321/saving-extracted-sap-report-in-excel-format-into-s.html
        public void exportSpreadsheet(GuiSession session) {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);
            guiGridView.PressToolbarContextButton("&MB_EXPORT");
            guiGridView.SelectContextMenuItem("&XXL");
        }

        public void selectRow(int rowIdx, GuiSession session)
        {
             GuiGridView guiGridView = (GuiGridView)session.FindById(id);
             guiGridView.CurrentCellRow = rowIdx - 1;
             guiGridView.SelectedRows = $"{rowIdx - 1}";
        }
    }
}