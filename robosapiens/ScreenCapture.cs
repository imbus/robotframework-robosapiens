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

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        // This is necessary when the SAP window contains an embedded Edge browser
        // Without this flag the embedded browser is absent in the screenshot
        const UInt32 PW_RENDERFULLCONTENT = 0x00000002;
        const UInt32 SRCCOPY = 0x00CC0020;

        public static byte[] saveWindowImage(IntPtr windowHandle, bool screenshot)
        {
            var rect = new Rect();
            var src = GetDC(IntPtr.Zero);
            GetWindowRect(windowHandle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            using (var stream = new MemoryStream())
            {
                IntPtr deviceContext = graphics.GetHdc();
                if (screenshot) {
                    BitBlt(deviceContext, 0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top, src, rect.Left, rect.Top, SRCCOPY);
                }
                else {
                    PrintWindow(windowHandle, deviceContext, PW_RENDERFULLCONTENT);
                }
                graphics.ReleaseHdc(deviceContext);
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}