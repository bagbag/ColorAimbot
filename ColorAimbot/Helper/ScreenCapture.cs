using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ColorAimbot.Helper
{
    public static class ScreenCapture
    {
        public static Bitmap Screenshot( int width = 0, int height = 0, bool onlyForegroundWindow = false)
        {
            if (onlyForegroundWindow)
                return Screenshot(User32.GetForegroundWindow(), width, height);

            return Screenshot(new Rectangle(0, 0, (int) System.Windows.SystemParameters.PrimaryScreenWidth, (int) System.Windows.SystemParameters.PrimaryScreenHeight));
        }

        public static Bitmap Screenshot(IntPtr handle,  int width = 0, int height = 0)
        {
            RECT rect;
            User32.GetWindowRect(new HandleRef(new object(), handle), out rect);
            var windowRectangle = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

            Rectangle rectangle;
            if (width != 0 && height != 0)
                rectangle = new Rectangle(windowRectangle.X + (int)(windowRectangle.Width / 2f - width / 2f), windowRectangle.Y + (int)(windowRectangle.Height / 2f - height / 2f), width, height);
            else
                rectangle = windowRectangle;

            return Screenshot(rectangle);
        }

        public static Bitmap Screenshot(string windowName,  int width = 0, int height = 0)
        {
            return Screenshot(User32.FindWindow(windowName), width, height);
        }

        public static Bitmap Screenshot(Rectangle rectangle)
        {
            if (rectangle.Width == 0 || rectangle.Height == 0)
                return null;

            Bitmap screenshot = new Bitmap(rectangle.Width, rectangle.Height);
            Graphics g = Graphics.FromImage(screenshot);
            g.CopyFromScreen(rectangle.Left, rectangle.Top, 0, 0, screenshot.Size);
            g.Dispose();

            return screenshot;
        }
    }
}
