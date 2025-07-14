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

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);


        public static byte[] saveWindowImage(IntPtr windowHandle)
        {
            var rect = new Rect();
            var src = GetDC(IntPtr.Zero);
            GetWindowRect(windowHandle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var screenshot = new Bitmap(bounds.Width, bounds.Height);
            using (var graphics = Graphics.FromImage(screenshot))
            using (var stream = new MemoryStream())
            {
                IntPtr deviceContext = graphics.GetHdc();
                BitBlt(deviceContext, 0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top, src, 0, 0, 0x00CC0020);
                graphics.ReleaseHdc(deviceContext);
                screenshot.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}