namespace WinAppDriver.UI
{
    using System;
    using WinAppDriver.WinUserWrapper;

    internal class WindowFactory : IWindowFactory
    {
        private IUIAutomation uiAutomation;

        private IKeyboard keyboard;

        private IWinUserWrap winUserWrap;

        public WindowFactory(IUIAutomation uiAutomation, IKeyboard keyboard, IWinUserWrap winUserWrap)
        {
            this.uiAutomation = uiAutomation;
            this.keyboard = keyboard;
            this.winUserWrap = winUserWrap;
        }

        public IWindow GetWindow(IntPtr handle)
        {
            return new Window(handle, this.uiAutomation, this.keyboard, this.winUserWrap);
        }
    }
}