namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;

    [Route("GET", "/session/:sessionId/element/:id/location")]
    internal class GetElementLocationHandler : IHandler
    {
        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));

            var rect = element.Current.BoundingRectangle;
            return new Dictionary<string, int> {
                { "x", (int)rect.X },
                { "y", (int)rect.Y }
            };
        }
    }
}