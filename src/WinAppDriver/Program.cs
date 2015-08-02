namespace WinAppDriver
{
    using System;
    using SystemWrapper.Windows.Input;
    using WinAppDriver.Handlers;
    using WinAppDriver.UAC;
    using WinAppDriver.UI;
    using WinAppDriver.WinUserWrapper;

    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var sessionMgr = new SessionManager();
            IOverlay overlay = new Overlay();
            var requestMgr = InitRequestManager(sessionMgr, overlay);

            overlay.Open();
            new Server(requestMgr).Start();
        }

        private static RequestManager InitRequestManager(SessionManager sessionManager, IOverlay overlay)
        {
            var context = new DriverContext();
            var manager = new RequestManager(sessionManager);
            var utils = new Utils();
            var winUserWrap = new WinUserWrap();
            var keyboard = new Keyboard(new KeyboardWrap(), new KeyInteropWrap(), winUserWrap);
            IMouse mouse = new Mouse(winUserWrap);
            var wireKeyboard = new WireKeyboard(keyboard);

            // TODO circular dependency, bad smell?
            IUIAutomation uiAutomation = new UIAutomation();
            IElementFactory elementFactory = new ElementFactory();
            ((UIAutomation)uiAutomation).SetElementFactory(elementFactory);
            ((ElementFactory)elementFactory).SetUIAutomation(uiAutomation);

            var uacHandler = new UACPromptHandler(uiAutomation, keyboard);
            var windowFactory = new WindowFactory(uiAutomation, keyboard, winUserWrap);
            var windowUtils = new WindowUtils(uiAutomation, windowFactory);

            manager.AddHandler(new ButtonUpHandler(mouse));
            manager.AddHandler(new ButtonDownHandler(mouse));
            manager.AddHandler(new ClearTextHandler(elementFactory));
            manager.AddHandler(new ClickElementHandler(mouse, overlay, elementFactory));
            manager.AddHandler(new ClickHandler(mouse));
            manager.AddHandler(new CloseWindowHandler(windowUtils));
            manager.AddHandler(new DeleteSessionHandler(sessionManager));
            manager.AddHandler(new DoubleClickHandler(mouse));
            manager.AddHandler(new FindElementHandler(uiAutomation, overlay, elementFactory));
            manager.AddHandler(new FindElementsHandler(uiAutomation, overlay, elementFactory));
            manager.AddHandler(new GetElementAttributeHandler(elementFactory));
            manager.AddHandler(new GetElementLocationHandler(elementFactory));
            manager.AddHandler(new GetElementLocationInViewHandler(elementFactory));
            manager.AddHandler(new GetElementSizeHandler(elementFactory));
            manager.AddHandler(new GetElementTagNameHandler());
            manager.AddHandler(new GetElementTextHandler());
            manager.AddHandler(new GetCurrentWindowHandler(uiAutomation));
            manager.AddHandler(new GetSourceHandler(uiAutomation));
            manager.AddHandler(new GetTitleHandler(uiAutomation));
            manager.AddHandler(new GetWindowLocationHandler(windowFactory, windowUtils));
            manager.AddHandler(new GetWindowsHandler(windowUtils));
            manager.AddHandler(new GetWindowSizeHandler(windowFactory, windowUtils));
            manager.AddHandler(new IsElementDisplayedHandler());
            manager.AddHandler(new IsElementEnabledHandler());
            manager.AddHandler(new IsElementSelectedHandler());
            manager.AddHandler(new MoveToHandler(mouse, elementFactory));
            manager.AddHandler(new NewSessionHandler(context, sessionManager, uacHandler, utils));
            manager.AddHandler(new ScreenshotHandler());
            manager.AddHandler(new SendKeysHandler(wireKeyboard));
            manager.AddHandler(new SetElementValueHandler(wireKeyboard, elementFactory));
            manager.AddHandler(new SwitchToWindowHandler(uiAutomation, windowFactory, windowUtils));

            return manager;
        }
    }
}
