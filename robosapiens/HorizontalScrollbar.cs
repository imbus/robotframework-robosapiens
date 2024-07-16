using sapfewse;

namespace RoboSAPiens 
{
    public class HorizontalScrollbar
    {
        string userAreaId;

        public HorizontalScrollbar(GuiUserArea guiUserArea)
        {
            userAreaId = guiUserArea.Id;
        }

        public bool scroll(GuiSession session, string toPosition)
        {
            var userArea = (GuiUserArea)session.FindById(userAreaId);
            var horizontalScrollbar = userArea.HorizontalScrollbar;

            if (toPosition == "BEGIN")
            {
                horizontalScrollbar.Position = horizontalScrollbar.Minimum;
                return true;
            }

            if (toPosition == "END")
            {
                horizontalScrollbar.Position = horizontalScrollbar.Maximum;
                return true;
            }

            if (toPosition == "RIGHT" && horizontalScrollbar.Position < horizontalScrollbar.Maximum)
            {
                horizontalScrollbar.Position++;
                return true;
            }

            if (toPosition == "LEFT" && horizontalScrollbar.Position > horizontalScrollbar.Minimum)
            {
                horizontalScrollbar.Position--;
                return true;
            }

            return false;
        }
    }
}
