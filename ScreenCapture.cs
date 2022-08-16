using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RoboSAPiens {
    public class ScreenCapture {
        [StructLayout(LayoutKind.Sequential)]
        struct Rect {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }   

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
        const int SW_MAXIMIZE = 3;


        private static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        public static bool windowIsMaximized(IntPtr windowHandle) {
            return GetPlacement(windowHandle).showCmd == ShowWindowCommands.Maximized;
        }

        public static void maximizeWindow(IntPtr windowHandle) {
            ShowWindow(windowHandle, SW_MAXIMIZE);
        }

        public static void saveWindowImage(IntPtr windowHandle, string filePath) {
            SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            var rect = new Rect();
            GetWindowRect(windowHandle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var screenshot = new Bitmap(bounds.Width, bounds.Height);
            using (var graphics = Graphics.FromImage(screenshot)) {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }
            screenshot.Save(filePath);
        }
    }
}