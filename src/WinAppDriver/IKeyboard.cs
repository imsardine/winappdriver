namespace WinAppDriver
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Input;

    internal interface IKeyboard
    {
        void ShowCharmsMenu();
    }

    internal class KeyboardImpl : IKeyboard
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        public void ShowCharmsMenu()
        {
            WindowsAPI.INPUT win_down = new WindowsAPI.INPUT
            {
                type = WindowsAPI.INPUT_KEYBOARD,
                u = new WindowsAPI.InputBatch
                {
                    ki = new WindowsAPI.KEYBDINPUT
                    {
                        wVk = 0x5B,
                        wScan = 0,
                        dwFlags = (int)WindowsAPI.KEYEVENTF.KEYDOWN,
                        dwExtraInfo = WindowsAPI.GetMessageExtraInfo(),
                    }
                }

            };

            WindowsAPI.INPUT win_up = new WindowsAPI.INPUT
            {
                type = WindowsAPI.INPUT_KEYBOARD,
                u = new WindowsAPI.InputBatch
                {
                    ki = new WindowsAPI.KEYBDINPUT
                    {
                        wVk = 0x5B,
                        wScan = 0,
                        dwFlags = (int)WindowsAPI.KEYEVENTF.KEYUP,
                        dwExtraInfo = WindowsAPI.GetMessageExtraInfo(),
                    }
                }

            };

            WindowsAPI.INPUT c_key = new WindowsAPI.INPUT
            {
                type = WindowsAPI.INPUT_KEYBOARD,
                u = new WindowsAPI.InputBatch
                {
                    ki = new WindowsAPI.KEYBDINPUT
                    {
                        wVk = 0x43,
                        wScan = 0,
                        dwFlags = (int)WindowsAPI.KEYEVENTF.KEYDOWN,
                        dwExtraInfo = WindowsAPI.GetMessageExtraInfo(),
                    }
                }

            };

            WindowsAPI.SendInput(2, new WindowsAPI.INPUT[] { win_down, c_key }, Marshal.SizeOf(typeof(WindowsAPI.INPUT)));


            logger.Info("WIN state: {0}", (Keyboard.GetKeyStates(Key.LWin) & KeyStates.Down));
            WindowsAPI.SendInput(1, new WindowsAPI.INPUT[] { win_up }, Marshal.SizeOf(typeof(WindowsAPI.INPUT)));
            logger.Info("WIN state: {0}", (Keyboard.Modifiers & ModifierKeys.Windows));
        }

        private class WindowsAPI
        {
            internal static int INPUT_MOUSE = 0;

            internal static int INPUT_KEYBOARD = 1;

            internal static int INPUT_HARDWARE = 2;

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetMessageExtraInfo();

            [Flags]
            internal enum KEYEVENTF
            {
                KEYDOWN = 0,
                EXTENDEDKEY = 0x0001,
                KEYUP = 0x0002,
                UNICODE = 0x0004,
                SCANCODE = 0x0008,
            }

            internal struct INPUT
            {
                public int type;
                public InputBatch u;
            }

            [StructLayout(LayoutKind.Explicit)]
            internal struct InputBatch
            {
                [FieldOffset(0)]
                public MOUSEINPUT mi;

                [FieldOffset(0)]
                public KEYBDINPUT ki;

                [FieldOffset(0)]
                public HARDWAREINPUT hi;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct MOUSEINPUT
            {
                public int dx;
                public int dy;
                public uint mouseData;
                public uint dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct KEYBDINPUT
            {
                public ushort wVk;
                public ushort wScan;
                public uint dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct HARDWAREINPUT
            {
                public uint uMsg;
                public ushort wParamL;
                public ushort wParamH;
            }

        }

    }

}
