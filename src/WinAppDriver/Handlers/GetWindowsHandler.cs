namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("GET", "/session/:sessionId/window_handles")]
    internal class GetWindowsHandler : IHandler
    {
        private IWindowUtils windowUtils;

        public GetWindowsHandler(IWindowUtils windowUtils)
        {
            this.windowUtils = windowUtils;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var handles = new HashSet<string>();
            foreach (var handle in this.windowUtils.GetTopLevelWindowHandles())
            {
                handles.Add(handle.ToString());
            }

            return handles;
        }
    }
}