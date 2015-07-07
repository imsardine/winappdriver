namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Automation;

    [Route("GET", "/session/:sessionId/element/:id/selected")]
    internal class IsElementSelectedHandler : IHandler
    {
        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));

            object pattern;
            if (element.TryGetCurrentPattern(TogglePattern.Pattern, out pattern))
            {
                var state = ((TogglePattern)pattern).Current.ToggleState;
                return state != ToggleState.Off;
            }
            else
            {
                return false;
            }
        }
    }
}