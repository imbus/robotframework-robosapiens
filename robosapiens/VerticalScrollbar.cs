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

        public bool scroll(GuiSession session, string toPosition)
        {
            var userArea = (GuiUserArea)session.FindById(userAreaId);
            var verticalScrollbar = userArea.VerticalScrollbar;

            if (toPosition == "BEGIN")
            {
                verticalScrollbar.Position = verticalScrollbar.Minimum;
                return true;
            }

            if (toPosition == "END")
            {
                verticalScrollbar.Position = verticalScrollbar.Maximum;
                return true;
            }

            if (toPosition == "DOWN" && verticalScrollbar.Position < verticalScrollbar.Maximum)
            {
                verticalScrollbar.Position++;
                return true;
            }

            if (toPosition == "UP" && verticalScrollbar.Position > verticalScrollbar.Minimum)
            {
                verticalScrollbar.Position--;
                return true;
            }

            return false;
        }
    }
}
