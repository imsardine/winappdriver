namespace WinAppDriver
{
    using System.Runtime.InteropServices;
    using System.Windows.Input;
    using SystemWrapper.Windows.Input;
    using WinUserWrapper;

    internal class Keyboard : IKeyboard
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IKeyboardWrap keyboard;

        private IWinUserWrap winUser;

        public Keyboard(IKeyboardWrap keyboard, IWinUserWrap winUser)
        {
            this.keyboard = keyboard;
            this.winUser = winUser;
        }

        public void ShowCharmsMenu()
        {
            INPUT win_down = new INPUT
            {
                type = (int)INPUTTYPE.KEYBOARD,
                ki = new KEYBDINPUT
                {
                    wVk = 0x5B,
                    wScan = 0,
                    dwFlags = (int)KEYEVENTF.KEYDOWN,
                    dwExtraInfo = winUser.GetMessageExtraInfo(),
                }
            };

            INPUT win_up = new INPUT
            {
                type = (int)INPUTTYPE.KEYBOARD,
                ki = new KEYBDINPUT
                {
                    wVk = 0x5B,
                    wScan = 0,
                    dwFlags = (int)KEYEVENTF.KEYUP,
                    dwExtraInfo = winUser.GetMessageExtraInfo(),
                }
            };

            INPUT c_key = new INPUT
            {
                type = (int)INPUTTYPE.KEYBOARD,
                ki = new KEYBDINPUT
                {
                    wVk = 0x43,
                    wScan = 0,
                    dwFlags = (int)KEYEVENTF.KEYDOWN,
                    dwExtraInfo = winUser.GetMessageExtraInfo(),
                }
            };

            winUser.SendInput(2, new INPUT[] { win_down, c_key }, Marshal.SizeOf(typeof(INPUT)));

            logger.Info("WIN state: {0}", (keyboard.GetKeyStates(Key.LWin) & KeyStates.Down));
            winUser.SendInput(1, new INPUT[] { win_up }, Marshal.SizeOf(typeof(INPUT)));
            logger.Info("WIN state: {0}", (keyboard.GetKeyStates(Key.LWin) & KeyStates.Down));
        }
    }
}