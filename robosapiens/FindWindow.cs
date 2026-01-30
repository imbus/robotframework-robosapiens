using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace RoboSAPiens
{
    public class FindWindow
    {
        public delegate bool EnumChildWindowsProc(IntPtr hWnd, IntPtr lParam);
        
        [DllImport("user32")]
        public static extern bool EnumChildWindows(IntPtr hWnd, EnumChildWindowsProc callback, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public static bool findWindow(int processId)
        {
            int elapsed = 0;
            const int timeout = 5000;
            const int wait = 500;
            bool windowFound = false;

            while (elapsed <= timeout)
            {
                // EnumChildWindows continues until the last child window is enumerated or the callback function returns false.
                EnumChildWindows(
                    GetDesktopWindow(), 
                    (hWnd, lParam) =>
                    {
                        uint windowProcId;
                        GetWindowThreadProcessId(hWnd, out windowProcId);

                        if (windowProcId == processId)
                        {
                            windowFound = true;
                            return false;
                        }
                        
                        return true;
                    }, 
                    0
                );

                if (!windowFound)
                {
                    Thread.Sleep(wait);
                    elapsed += wait;                  
                }
                else
                {
                    break;
                }
            }

            return windowFound;
        }
    }
}
