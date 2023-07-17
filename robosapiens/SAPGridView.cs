using sapfewse;

namespace RoboSAPiens {
    public class SAPGridView {
        string id;

        public SAPGridView(GuiGridView guiGridView) {
            id = guiGridView.Id;
        }

        public void exportSpreadsheet(GuiSession session) {
            GuiGridView guiGridView = (GuiGridView)session.FindById(id);
            guiGridView.PressToolbarContextButton("&MB_EXPORT");
            guiGridView.SelectContextMenuItem("&XXL");
        }
    }
}