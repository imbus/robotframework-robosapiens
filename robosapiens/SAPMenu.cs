using System;
using sapfewse;

namespace RoboSAPiens
{
    public class SAPMenu: ILabeled
    {
        string id;
        string path;
        string tooltip;

        public SAPMenu(GuiMenu guiMenu, String path)
        {
            id = guiMenu.Id;
            tooltip = guiMenu.DefaultTooltip;
            this.path = path;
        }

        public bool hasTooltip(string tooltip)
        {
            return this.tooltip == tooltip;
        }

        public bool isHLabeled(string label)
        {
            return path == label;
        }

        public bool isVLabeled(string label)
        {
            return false;
        }

        public void select(GuiSession session)
        {
            var guiMenu = (GuiMenu)session.FindById(id);
            guiMenu.Select();
        }
    }
}
