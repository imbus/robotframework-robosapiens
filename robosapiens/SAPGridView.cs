using sapfewse;

namespace RoboSAPiens {
    public class SAPGridView {
        string id;

        public SAPGridView(GuiGridView guiGridView) {
            id = guiGridView.Id;
        }

        // https://answers.sap.com/questions/290321/saving-extracted-sap-report-in-excel-format-into-s.html
        public void exportSpreadsheet(GuiSession session) {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);
            guiGridView.PressToolbarContextButton("&MB_EXPORT");
            guiGridView.SelectContextMenuItem("&XXL");
        }
    }
}