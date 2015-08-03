namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [Route("POST", "/session/:sessionId/keys")]
    internal class SendKeysHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IWireKeyboard keyboard;

        public SendKeysHandler(IWireKeyboard keyboard)
        {
            this.keyboard = keyboard;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var request = JsonConvert.DeserializeObject<ElementValueRequest>(body);
            var keys = string.Join(string.Empty, request.KeySequence);
            logger.Debug("Keys: {0}", keys);

            this.keyboard.SendKeys(keys.ToCharArray(), session.Capabilities.KeystrokeDelay);

            return null;
        }

        private class ElementValueRequest
        {
            [JsonProperty("value")]
            internal string[] KeySequence { get; set; }
        }
    }
}