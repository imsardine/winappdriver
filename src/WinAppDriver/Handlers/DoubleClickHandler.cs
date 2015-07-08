namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/doubleclick")]
    internal class DoubleClickHandler : IHandler
    {
        private IMouse mouse;

        public DoubleClickHandler(IMouse mouse)
        {
            this.mouse = mouse;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            this.mouse.DoubleClick();
            return null;
        }
    }
}