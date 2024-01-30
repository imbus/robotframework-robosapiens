using sapfewse;

namespace RoboSAPiens 
{
    public class VerticalScrollbar
    {
        string userAreaId;

        public VerticalScrollbar(GuiUserArea guiUserArea)
        {
            userAreaId = guiUserArea.Id;
        }

        public bool scroll(GuiSession session, string targetForm)
        {
            var userArea = (GuiUserArea)session.FindById(userAreaId);
            var verticalScrollbar = userArea.VerticalScrollbar;

            if (targetForm == "FIRST")
            {
                verticalScrollbar.Position = verticalScrollbar.Minimum;
                return true;
            }

            if (targetForm == "LAST")
            {
                verticalScrollbar.Position = verticalScrollbar.Maximum;
                return true;
            }

            if (targetForm == "NEXT" && verticalScrollbar.Position < verticalScrollbar.Maximum)
            {
                verticalScrollbar.Position++;
                return true;
            }

            if (targetForm == "PREV" && verticalScrollbar.Position > verticalScrollbar.Minimum)
            {
                verticalScrollbar.Position--;
                return true;
            }

            return false;
        }
    }
}
