using SAPFEWSELib;
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

        public SAPWindow(GuiFrameWindow window, bool loadComponents=false) {
            components = loadComponents ? new Components(window.Children) : new Components();
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
            var message = String.Join(" ", components.getAllTextFields()
                                                     .Select(field => field.text));
            return $"{title}: {message}";
        }

        public bool isErrorWindow() {
            return windowType == "GuiModalWindow" && 
            (title.Contains("Fehler") || title.Contains("Mehrfachanmeldung"));
        }

        public bool isInfoWindow() {
            return windowType == "GuiModalWindow" && title.Contains("Information");
        }
        
        public bool isModalWindow() {
            return windowType == "GuiModalWindow";
        }

        public void pressKey(int key) {
            self.SendVKey(key);
        }

        public void saveScreenshot(string filePath) {
            ScreenCapture.saveWindowImage(windowHandle, filePath);
        }
    }
}
