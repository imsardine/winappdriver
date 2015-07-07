namespace WinAppDriver.Handlers
{
    using System;
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("GET", "/session/:sessionId/window/:windowHandle/size")]
    internal class GetWindowSizeHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IWindowFactory windowFactory;

        private IWindowUtils windowUtils;

        public GetWindowSizeHandler(IWindowFactory windowFactory, IWindowUtils windowUtils)
        {
            this.windowFactory = windowFactory;
            this.windowUtils = windowUtils;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var handle = urlParams["windowHandle"];

            IWindow window = null;
            if (handle == "current")
            {
                window = this.windowUtils.GetCurrentWindow();
            }
            else
            {
                this.windowFactory.GetWindow(new IntPtr(int.Parse(handle)));
            }

            var size = window.Size;
            return new Dictionary<string, int>
            {
                { "width", (int)size.Width },
                { "height", (int)size.Height }
            };
        }
    }
}