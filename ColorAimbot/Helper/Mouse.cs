using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;
using WindowsInput;

namespace ColorAimbot.Helper
{
    static class Mouse
    {
        public static Vector Position
        {
            get
            {
                return User32.GetCursorPos();
            }
            set
            {
                User32.SetCursorPos((int)value.X, (int)value.Y);
            }
        }

        public static void Move(Vector offset)
        {
            //Position = Vector.Add(Position, offset);

            new InputSimulator().Mouse.MoveMouseBy((int)offset.X, (int)offset.Y);
        }
    }
}
