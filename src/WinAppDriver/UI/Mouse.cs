namespace WinAppDriver.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Input;
    using WinAppDriver.WinUserWrapper;

    internal class Mouse : IMouse
    {
        private const int NormalizedMaximum = 0xFFFF;

        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IWinUserWrap winUser;

        public Mouse(IWinUserWrap winUser)
        {
            this.winUser = winUser;
        }

        public Point Position
        {
            get
            {
                return System.Windows.Forms.Cursor.Position;
            }

            private set
            {
                Point normalizedXY = this.NormalizeCoordinates(value.X, value.Y);

                INPUT input = new INPUT
                {
                    type = (int)INPUTTYPE.MOUSE,
                    u = new InputUnion
                    {
                        mi = new MOUSEINPUT
                        {
                            dx = normalizedXY.X,
                            dy = normalizedXY.Y,
                            mouseData = 0,
                            dwFlags = (uint)(MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.VIRTUALDESK),
                            time = 0,
                            dwExtraInfo = new IntPtr(0),
                        }
                    }
                };
                this.winUser.SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
            }
        }

        public Point NormalizeCoordinates(int x, int y)
        {
            var width = System.Windows.Forms.SystemInformation.VirtualScreen.Width;
            var height = System.Windows.Forms.SystemInformation.VirtualScreen.Height;
            int normalizedX = (NormalizedMaximum * x) / width;
            int normalizedY = (NormalizedMaximum * y) / height;
            return new Point(normalizedX, normalizedY);
        }

        public void Click(MouseButton button)
        {
            this.Down(button);
            this.Up(button);
        }

        public void DoubleClick()
        {
            this.Click(MouseButton.Left);
            this.Click(MouseButton.Left);
        }

        public void Move(int x, int y)
        {
            Point pos = this.Position;
            this.MoveTo(pos.X + x, pos.Y + y);
        }

        public void MoveTo(int x2, int y2)
        {
            Point pos = this.Position;
            int x = pos.X;
            int y = pos.Y;

            var msg = new System.Text.StringBuilder();
            msg.AppendFormat(
                "Move the mouse pointer; from = [{0},{1}], to = [{2},{3}]",
                x, y, x2, y2);

            // duration determins speed, and steps determins smoothness
            int duration = 200;
            int steps = 20;
            int interval = duration / steps;
            int stepX = (x2 - x) / steps;
            int stepY = (y2 - y) / steps;
            msg.AppendFormat(
                ", duration = {0} ms, steps = {1}, interval = {2} ms, step = [{3},{4}]",
                duration, steps, interval, stepX, stepY);

            msg.Append(", movements = ");
            var movements = new List<Point>();
            for (int i = 0; i < steps; i++)
            {
                x += stepX;
                y += stepY;
                msg.AppendFormat("[{0},{1}]", x, y);
                movements.Add(new Point(x, y));
            }

            if (x != x2 || y != y2)
            {
                msg.AppendFormat("[{0},{1}]", x2, y2);
                movements.Add(new Point(x2, y2));
            }

            logger.Debug(msg.ToString());

            foreach (var point in movements)
            {
                this.Position = point;
                Thread.Sleep(interval);
            }
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