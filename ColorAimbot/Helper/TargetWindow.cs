using System;
using System.Drawing;
using System.Timers;

namespace ColorAimbot.Helper
{
    public  class TargetWindow
    {
        private readonly Settings settings;
        private readonly Timer timer;
        private  FindType findType;

        private  IntPtr windowHandle;
        private  Rectangle windowRectangle;
        private  System.Drawing.Point windowCenter;
        private  string windowTitle;
        private  bool foundWindow;
        private  bool isInForeground;
        private  Vector offset;

        public IntPtr WindowHandle { get { return windowHandle; } }

        public Rectangle WindowRectangle { get { return windowRectangle; } }

        public System.Drawing.Point WindowCenter { get { return windowCenter; } }

        public Vector Offset { get { return offset; } }

        public string WindowTitle { get { return windowTitle; } }

        public bool FoundWindow { get { return foundWindow; } }

        public bool IsInForeground { get { return isInForeground; } }


        public  Vector CursorPosition
        {
            get
            {
                if (!foundWindow)
                    return new Vector(double.NaN, double.NaN);

                var realPosition = User32.GetCursorPos();

                return Vector.Subtract(realPosition, offset);
            }
        }

         public TargetWindow(Settings settings)
         {
             this.settings = settings;

            timer = new Timer();
            timer.Elapsed += (sender, args) => timerCallback();
            timer.Interval = 1000;

            UseForegroundWindow();

            timer.Start();
        }

        ~TargetWindow()
        {
            timer.Stop();
        }

        public  void UseWindowHandle(IntPtr windowHandle)
        {
            this.windowHandle = windowHandle;
            findType = FindType.Handle;

            timerCallback();
        }

        public  void UseWindowTitle(string windowTitle)
        {
            this.windowTitle = windowTitle;
            findType = FindType.Title;

            timerCallback();
        }

        public  void UseForegroundWindow()
        {
            findType = FindType.Foreground;

            timerCallback();
        }

        private  void timerCallback()
        {
            switch (findType)
            {
                case FindType.Handle:

                    if (!User32.IsWindowVisible(windowHandle))
                    {
                        foundWindow = false;
                        break;
                    }

                    foundWindow = true;

                    GetWindowTitle();
                    GetWindowRect();

                    break;
                case FindType.Title:

                    windowHandle = User32.FindWindow(windowTitle);

                    if (windowHandle == IntPtr.Zero || !User32.IsWindowVisible(windowHandle))
                    {
                        foundWindow = false;
                        break;
                    }

                    foundWindow = true;

                    GetWindowTitle();
                    GetWindowRect();

                    break;
                case FindType.Foreground:

                    windowHandle = User32.GetForegroundWindow();

                    if (!User32.IsWindowVisible(windowHandle))
                    {
                        foundWindow = false;
                        break;
                    }

                    foundWindow = true;

                    GetWindowTitle();
                    GetWindowRect();

                    break;
            }

            GetIsInForeground();
        }

        private  void GetWindowTitle()
        {
            windowTitle = User32.GetWindowText(windowHandle);
        }

        private  bool GetWindowRect()
        {
            RECT clientRect;

            if (!User32.GetClientRect(windowHandle, out clientRect))
                return false;

            var clientPoint = new POINT(0, 0);
            User32.ClientToScreen(windowHandle, ref clientPoint);

            var rectangle = new Rectangle(clientPoint.X, clientPoint.Y, clientRect.Right, clientRect.Bottom);

            windowCenter = new System.Drawing.Point(rectangle.X + (int)(rectangle.Width / 2f), rectangle.Y + (int)(rectangle.Height / 2f));

            windowRectangle = new Rectangle(windowCenter.X - (int)(settings.SearchWidth / 2.0f), windowCenter.Y - (int)(settings.SearchHeight / 2.0f), settings.SearchWidth, settings.SearchHeight);

            offset = new Vector(windowRectangle.X, windowRectangle.Y);

            return true;
        }

        private  void GetIsInForeground()
        {
            if (windowHandle == User32.GetForegroundWindow())
                isInForeground = true;
            else
                isInForeground = false;
        }

        enum FindType
        {
            Handle,
            Title,
            Foreground
        }
    }
}
