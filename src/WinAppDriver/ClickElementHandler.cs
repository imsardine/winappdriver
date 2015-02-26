namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Automation;

    [Route("POST", "/session/:sessionId/element/:id/click")]
    internal class ClickElementHandler : IHandler
    {
        private const int MouseEventFlagMove = 0x0001;
        private const int MouseEventFlagLeftdown = 0x0002;
        private const int MouseEventFlagLeftup = 0x0004;
        private const int MouseEventFlagRightdown = 0x0008;
        private const int MouseEventFlagRightup = 0x0010;
        private const int MouseEventFlagMiddledown = 0x0020;
        private const int MouseEventFlagMiddleup = 0x0040;
        private const int MouseEventFlagAbsolute = 0x8000;

        private static ILogger logger = Logger.GetLogger("WinAppDriver");
        
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));
            Rect rect = element.Current.BoundingRectangle;

            int x = (int)(rect.Left + (rect.Width / 2));
            int y = (int)(rect.Top + (rect.Height / 2));

            logger.Info("click " + element.Current.Name);
            SetCursorPos(x, y);
            mouse_event(MouseEventFlagLeftdown, 0, 0, 0, 0);
            mouse_event(MouseEventFlagLeftup, 0, 0, 0, 0);
            return null;
        }

        [DllImport("user32.dll")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static extern void mouse_event(int mouseEventFlag, int incrementX, int incrementY, int data, int extraInfo);
    }
}