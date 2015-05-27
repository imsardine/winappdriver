namespace WinAppDriver.UI
{
    using System;
    using WinAppDriver.WinUserWrapper;

    internal class WindowFactory : IWindowFactory
    {
        private IKeyboard keyboard;

        private IWinUserWrap winUserWrap;

        public WindowFactory(IKeyboard keyboard, IWinUserWrap winUserWrap)
        {
            this.keyboard = keyboard;
            this.winUserWrap = winUserWrap;
        }

        public IWindow GetWindow(IntPtr handle)
        {
            return new Window(handle, this.keyboard, this.winUserWrap);
        }
    }
}