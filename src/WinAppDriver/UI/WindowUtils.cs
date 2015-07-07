namespace WinAppDriver.UI
{
    using System;
    using System.Collections.Generic;

    internal class WindowUtils : IWindowUtils
    {
        private IUIAutomation uiAutomation;

        private IWindowFactory windowFactory;

        public WindowUtils(IUIAutomation uiAutomation, IWindowFactory windowFactory)
        {
            this.uiAutomation = uiAutomation;
            this.windowFactory = windowFactory;
        }

        public IWindow GetCurrentWindow()
        {
            var handle = this.uiAutomation.ToNativeWindowHandle(
                this.uiAutomation.GetFocusedWindowOrRoot());
            return this.windowFactory.GetWindow(handle);
        }

        public ISet<IntPtr> GetTopLevelWindowHandles()
        {
            var handles = new HashSet<IntPtr>();
            foreach (var window in this.uiAutomation.GetTopLevelWindows())
            {
                var handle = this.uiAutomation.ToNativeWindowHandle(window);
                handles.Add(handle);
            }

            return handles;
        }

        public ISet<IWindow> GetTopLevelWindows()
        {
            var windows = new HashSet<IWindow>();
            foreach (var handle in this.GetTopLevelWindowHandles())
            {
                windows.Add(this.windowFactory.GetWindow(handle));
            }

            return windows;
        }
    }
}