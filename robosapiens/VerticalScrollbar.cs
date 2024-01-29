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

        public bool scroll(GuiSession session, string step)
        {
            var userArea = (GuiUserArea)session.FindById(userAreaId);
            var verticalScrollbar = userArea.VerticalScrollbar;

            if (step == "START")
            {
                verticalScrollbar.Position = verticalScrollbar.Minimum;
                return true;
            }

            if (step == "END")
            {
                verticalScrollbar.Position = verticalScrollbar.Maximum;
                return true;
            }

            if (step == "DOWN" && verticalScrollbar.Position < verticalScrollbar.Maximum)
            {
                verticalScrollbar.Position++;
                return true;
            }

            if (step == "UP" && verticalScrollbar.Position > verticalScrollbar.Minimum)
            {
                verticalScrollbar.Position--;
                return true;
            }

            return false;
        }
    }
}
