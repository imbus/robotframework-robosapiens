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