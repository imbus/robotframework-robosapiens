using sapfewse;
using System;
using System.Linq;
using System.Runtime.InteropServices;

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
                return string.Join("\n", 
                    components.getAllTextFields()
                    .Select(field => field.text)
                    .Where(text => text != "")
                );
            }

            if (components.getAllLabels().Count > 0) {
                return string.Join("\n", 
                    components.getAllLabels()
                    .Select(label => label.text)
                    .Where(text => text != "")
                );
            }

            return "";
        }

        public void maximize()
        {
            self.Maximize();
        }

        public void pressKey(int key)
        {
            self.SendVKey(key);
        }

		[DllImport("user32.dll", SetLastError = true)]
		static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        const int KEYEVENTF_KEYUP = 0x0002;
        const int WM_KEYDOWN = 0x0100;

        public void pressEnter()
        {
		    const int VK_RETURN  = 0x0D;
            PostMessage(self.Handle, WM_KEYDOWN, VK_RETURN, 0);
        }

        public void pressF1()
        {
		    const int VK_F1  = 0x70;
            
            SetForegroundWindow(self.Handle);
            keybd_event(VK_F1, 0, 0, 0);
            keybd_event(VK_F1, 0, KEYEVENTF_KEYUP, 0);
        }

        public void pressF4()
        {
		    const int VK_F4  = 0x73;
            
            SetForegroundWindow(self.Handle);
            keybd_event(VK_F4, 0, 0, 0);
            keybd_event(VK_F4, 0, KEYEVENTF_KEYUP, 0);
        }

        public void pressPageDown()
        {
		    const int VK_NEXT  = 0x22;
            
            SetForegroundWindow(self.Handle);
            keybd_event(VK_NEXT, 0, 0, 0);
            keybd_event(VK_NEXT, 0, KEYEVENTF_KEYUP, 0);
        }

        public void setStatusbar(GuiSession session, string statusbarId) {
            var statusbar = (GuiStatusbar)session.FindById(statusbarId);
            components.setStatusBar(statusbar);
        }
    }
}
