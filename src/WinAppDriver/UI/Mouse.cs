namespace WinAppDriver.UI
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Input;
    using WinAppDriver.WinUserWrapper;

    internal class Mouse : IMouse
    {
        private IWinUserWrap winUser;

        public Mouse(IWinUserWrap winUser)
        {
            this.winUser = winUser;
        }

        public void Click(MouseButton button)
        {
            this.Down(button);
            this.Up(button);
        }

        public void DoubleClick(MouseButton button)
        {
            this.Click(button);
            this.Click(button);
        }

        public void Move(int x, int y)
        {
            this.SendMouseInput(x, y, MOUSEEVENTF.MOVE); // TODO doesn't work?
        }

        public void MoveTo(int x, int y)
        {
            // SendMouseInput(x, y, MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE) doesn't work
            this.winUser.SetCursorPos(x, y);
        }

        public void Down(MouseButton button)
        {
            this.SendMouseInput(0, 0, this.ToEventFlags(button, true));
        }

        public void Up(MouseButton button)
        {
            this.SendMouseInput(0, 0, this.ToEventFlags(button, false));
        }

        private MOUSEEVENTF ToEventFlags(MouseButton button, bool down)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return down ? MOUSEEVENTF.LEFTDOWN : MOUSEEVENTF.LEFTUP;

                case MouseButton.Middle:
                    return down ? MOUSEEVENTF.MIDDLEDOWN : MOUSEEVENTF.MIDDLEUP;

                case MouseButton.Right:
                    return down ? MOUSEEVENTF.RIGHTDOWN : MOUSEEVENTF.RIGHTUP;

                default:
                    throw new ArgumentException("Invalid mouse button.");
            }
        }

        private void SendMouseInput(int x, int y, MOUSEEVENTF flags)
        {
            INPUT input = new INPUT
            {
                type = (int)INPUTTYPE.MOUSE,
                u = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dx = x,
                        dy = y,
                        mouseData = 0,
                        dwFlags = (uint)flags,
                        time = 0, // the system will provide its own time stamp.
                        dwExtraInfo = this.winUser.GetMessageExtraInfo(),
                    }
                }
            };

            this.winUser.SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}