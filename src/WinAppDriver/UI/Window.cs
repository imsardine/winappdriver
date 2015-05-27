namespace WinAppDriver.UI
{
    using System;
    using System.Windows.Input;
    using WinAppDriver.WinUserWrapper;

    internal class Window : IWindow
    {
        private IKeyboard keyboard;

        private IWinUserWrap winUserWrap;

        public Window(IntPtr handle, IKeyboard keyboard, IWinUserWrap winUserWrap)
        {
            this.Handle = handle;
            this.keyboard = keyboard;
            this.winUserWrap = winUserWrap;
        }

        public IntPtr Handle { get; private set; }

        public void BringToFront()
        {
            // The system automatically enables calls to SetForegroundWindow if the user presses the ALT key
            // http://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo.aspx
            try
            {
                this.keyboard.KeyUpOrDown(Key.LeftAlt);
                this.winUserWrap.SetForegroundWindow(this.Handle);
            }
            finally
            {
                this.keyboard.KeyUpOrDown(Key.LeftAlt);
            }
        }

        public void Close()
        {
            // Bring the window to front and close it by pressing Alt + F4
            // TODO this approach doesn't work with all windows, e.g. Prompt
            this.BringToFront();

            this.keyboard.KeyUpOrDown(Key.LeftAlt);
            this.keyboard.KeyPress(Key.F4);
            this.keyboard.KeyUpOrDown(Key.LeftAlt);
        }
    }
}