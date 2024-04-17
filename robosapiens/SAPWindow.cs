using sapfewse;
using System;
using System.Linq;

namespace RoboSAPiens 
{
    public class SAPWindow 
    {
        public Components components;
        public string id {get;}
        public Position position;
        public string title {get;}
        GuiFrameWindow self;

        public SAPWindow(GuiFrameWindow window, bool debug=false) 
        {
            components = new Components(window.Children, debug);
            id = window.Id;
            position = new Position(
                height: window.Height,
                left: window.ScreenLeft,
                top: window.ScreenTop,
                width: window.Width
            );
            title = window.Text;
            self = window;
        }

        public string getMessage() 
        {
            if (components.getAllTextFields().Count > 0) {
                return String.Join("\n", 
                    components.getAllTextFields().Select(field => field.text));                 
            }
            
            if (components.getAllLabels().Count > 0) {
                return String.Join("\n", 
                    components.getAllLabels().Select(label => label.text));                 
            }

            return "";
        }
        
        public void pressKey(int key) {
            self.SendVKey(key);
        }

        public void setStatusbar(GuiSession session, string statusbarId) {
            var statusbar = (GuiStatusbar)session.FindById(statusbarId);
            components.setStatusBar(statusbar);
        }
    }
}
