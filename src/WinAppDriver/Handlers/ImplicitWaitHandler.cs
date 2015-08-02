namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [Route("POST", "/session/:sessionId/timeouts/implicit_wait")]
    internal class ImplicitWaitHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var request = JsonConvert.DeserializeObject<ImplicitWaitRequest>(body);
            logger.Debug("Implicit wait for this session: {0} ms.", request.ImplicitWaitMillis);

            session.ImplicitWaitMillis = (int)request.ImplicitWaitMillis;
            return null;
        }

        private class ImplicitWaitRequest
        {
            [JsonProperty("ms")]
            internal float ImplicitWaitMillis { get; set; }
        }
    }
}