namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("GET", "/session/:sessionId/element/:id/location")]
    internal class GetElementLocationHandler : IHandler
    {
        private IElementFactory elementFactory;

        public GetElementLocationHandler(IElementFactory elementFactory)
        {
            this.elementFactory = elementFactory;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var id = int.Parse(urlParams["id"]);
            var element = this.elementFactory.GetElement(session.GetUIElement(id));

            return new Dictionary<string, int>
            {
                { "x", element.X },
                { "y", element.Y }
            };
        }
    }
}