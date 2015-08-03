namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/element/:id/value")]
    internal class SetElementValueHandler : IHandler
    {
        private IWireKeyboard keyboard;

        private IElementFactory elementFactory;

        public SetElementValueHandler(IWireKeyboard keyboard, IElementFactory elementFactory)
        {
            this.keyboard = keyboard;
            this.elementFactory = elementFactory;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var element = this.elementFactory.GetElement(session.GetUIElement(int.Parse(urlParams["id"])));
            var request = JsonConvert.DeserializeObject<ElementValueRequest>(body);

            var text = string.Join(string.Empty, request.KeySequence);

            element.SetFocus();
            this.keyboard.Type(text, session.Capabilities.KeystrokeDelay);

            return null;
        }

        private class ElementValueRequest
        {
            [JsonProperty("value")]
            internal string[] KeySequence { get; set; }
        }
    }
}