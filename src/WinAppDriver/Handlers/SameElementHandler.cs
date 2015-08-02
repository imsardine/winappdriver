namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;

    [Route("GET", " /session/:sessionId/element/:id/equals/:other")]
    internal class SameElementHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));
            var other = session.GetUIElement(int.Parse(urlParams["other"]));
            return element.Equals(other);
        }
    }
}