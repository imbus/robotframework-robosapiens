using sapfewse;
using System;
using System.Linq;

namespace RoboSAPiens {
    public class SAPWindow {
        public Components components;
        public string id {get;}
        public Position position;
        public string title {get;}
        GuiFrameWindow self;
        IntPtr windowHandle;
        string windowType;

        public SAPWindow(GuiFrameWindow window, GuiSession session, bool loadComponents=false) {
            components = loadComponents ? new Components(window.Children, session) : new Components(session);
            id = window.Id;
            position = new Position(
                height: window.Height,
                left: window.ScreenLeft,
                top: window.ScreenTop,
                width: window.Width
            );
            title = window.Text;
            self = window;
            windowHandle = new IntPtr(window.Handle);
            windowType = window.Type;
        }

        public string getMessage() {
            if (components.getAllTextFields().Count > 0) {
                return String.Join(" ", 
                    components.getAllTextFields().Select(field => field.text));                 
            }
            
            if (components.getAllLabels().Count > 0) {
                return String.Join(" ", 
                    components.getAllLabels().Select(label => label.getText()));                 
            }

            return "";
        }
        
        public bool isModalWindow() {
            return windowType == "GuiModalWindow";
        }

        public void pressKey(int key) {
            self.SendVKey(key);
        }
    }
}
