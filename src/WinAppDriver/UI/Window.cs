namespace WinAppDriver.UI
{
    using System;
    using System.Windows.Input;
    using WinAppDriver.WinUserWrapper;

    internal class Window : IWindow
    {
        private IntPtr handle;

        private IKeyboard keyboard;

        private IWinUserWrap winUserWrap;

        public Window(IntPtr handle, IKeyboard keyboard, IWinUserWrap winUserWrap)
        {
            this.handle = handle;
            this.keyboard = keyboard;
            this.winUserWrap = winUserWrap;
        }

        public void BringToFront()
        {
            // The system automatically enables calls to SetForegroundWindow if the user presses the ALT key
            // http://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo.aspx
            try
            {
                this.keyboard.KeyUpOrDown(Key.LeftAlt);
                this.winUserWrap.SetForegroundWindow(this.handle);
            }
            finally
            {
                this.keyboard.KeyUpOrDown(Key.LeftAlt);
            }
        }
    }
}