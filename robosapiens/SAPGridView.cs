using sapfewse;

namespace RoboSAPiens {
    public class SAPGridView: ITable {
        string id;
        int rowCount;

        public SAPGridView(GuiGridView guiGridView) {
            id = guiGridView.Id;
            rowCount = guiGridView.RowCount;
        }

        public int getNumRows(GuiSession session)
        {
            var gridView = (GuiGridView)session.FindById(id);
            return gridView.RowCount;
        }

        // https://answers.sap.com/questions/290321/saving-extracted-sap-report-in-excel-format-into-s.html
        public void exportSpreadsheet(GuiSession session) {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);
            guiGridView.PressToolbarContextButton("&MB_EXPORT");
            guiGridView.SelectContextMenuItem("&XXL");
        }

        public void pressKey(string key, GuiSession session) 
        {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);

            if (key == "F1") guiGridView.PressF1();
            if (key == "F4") guiGridView.PressF4();
        }

        public void selectRow(int rowIdx, GuiSession session)
        {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);
            guiGridView.CurrentCellRow = rowIdx;
            guiGridView.SelectedRows = $"{rowIdx}";
        }

        public bool scrollOnePage(GuiSession session)
        {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);

            if (guiGridView.FirstVisibleRow + guiGridView.VisibleRowCount < guiGridView.RowCount)
            {
                guiGridView.FirstVisibleRow += guiGridView.VisibleRowCount;
                return true;
            }

            return false;
        }
    }
}
