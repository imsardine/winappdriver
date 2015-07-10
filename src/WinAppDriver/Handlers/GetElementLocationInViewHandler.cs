namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("GET", "/session/:sessionId/element/:id/location_in_view")]
    internal class GetElementLocationInViewHandler : IHandler
    {
        private IElementFactory elementFactory;

        public GetElementLocationInViewHandler(IElementFactory elementFactory)
        {
            this.elementFactory = elementFactory;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var id = int.Parse(urlParams["id"]);
            var element = this.elementFactory.GetElement(session.GetUIElement(id));

            element.ScrollIntoView();

            return new Dictionary<string, int>
            {
                { "x", element.X },
                { "y", element.Y }
            };
        }
    }
}