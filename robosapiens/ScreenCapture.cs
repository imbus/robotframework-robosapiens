using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace RoboSAPiens 
{
    public class ScreenCapture 
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Rect 
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }   

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        // This is necessary when the SAP window contains an embedded Edge browser
        // Without this flag the embedded browser is absent in the screenshot
        const UInt32 PW_RENDERFULLCONTENT = 0x00000002;

        public static byte[] saveWindowImage(IntPtr windowHandle)
        {
            var rect = new Rect();
            GetWindowRect(windowHandle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var screenshot = new Bitmap(bounds.Width, bounds.Height);
            using (var graphics = Graphics.FromImage(screenshot))
            using (var stream = new MemoryStream())
            {
                IntPtr deviceContext = graphics.GetHdc();
                bool success = PrintWindow(windowHandle, deviceContext, PW_RENDERFULLCONTENT);
                graphics.ReleaseHdc(deviceContext);
                screenshot.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}