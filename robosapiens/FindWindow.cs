using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace RoboSAPiens
{
    public class FindWindow
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        public delegate bool EnumChildWindowsProc(IntPtr hWnd, IntPtr lParam);
        
        [DllImport("user32")]
        public static extern bool EnumChildWindows(IntPtr hWnd, EnumChildWindowsProc callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int cch);
        
        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern Int32 GetWindowTextLength(IntPtr hWnd);

        public static string GetTitle(IntPtr hWnd)
        {
            var len = GetWindowTextLength(hWnd);
            StringBuilder title = new StringBuilder(len + 1);
            GetWindowText(hWnd, title, title.Capacity);
            return title.ToString();
        }

        public static bool findWindow(string windowTitle)
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
                        if (GetTitle(hWnd).StartsWith(windowTitle))
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
