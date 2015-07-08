namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/element/:id/clear")]
    internal class ClearTextHandler : IHandler
    {
        private IElementFactory elementFactory;

        public ClearTextHandler(IElementFactory elementFactory)
        {
            this.elementFactory = elementFactory;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var id = int.Parse(urlParams["id"]);
            var element = this.elementFactory.GetElement(session.GetUIElement(id));

            element.Value = string.Empty;

            return null;
        }
    }
}