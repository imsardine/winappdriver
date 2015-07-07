namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;

    [Route("GET", "/session/:sessionId/element/:id/enabled")]
    internal class IsElementEnabledHandler : IHandler
    {
        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));
            return element.Current.IsEnabled;
        }
    }
}