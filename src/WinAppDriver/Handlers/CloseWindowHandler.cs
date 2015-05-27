namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("DELETE", "/session/:sessionId/window")]
    internal class CloseWindowHandler : IHandler
    {
        private IWindowUtils windowsUtils;

        public CloseWindowHandler(IWindowUtils windowsUtils)
        {
            this.windowsUtils = windowsUtils;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            this.windowsUtils.GetCurrentWindow().Close();
            return null;
        }
    }
}