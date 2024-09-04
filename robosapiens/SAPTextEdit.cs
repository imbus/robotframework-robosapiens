using sapfewse;

namespace RoboSAPiens
{
    public class SAPTextEdit: IHighlightable
    {
        bool focused;
        string id;

        public SAPTextEdit(GuiTextedit textedit)
        {
            id = textedit.Id;
        }

        public bool isChangeable(GuiSession session)
        {
            var textEdit = (GuiTextedit)session.FindById(id);
            return textEdit.Changeable;
        }

        public void insert(GuiSession session, string content)
        {
            var textEdit = (GuiTextedit)session.FindById(id);
            textEdit.Text = content;
        }

        public void toggleHighlight(GuiSession session)
        {
            var textEdit = (GuiTextedit)session.FindById(id);
            focused = !focused;
            textEdit.Visualize(focused);
        }
    }
}
