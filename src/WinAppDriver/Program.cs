namespace WinAppDriver {

    class Program {

        static void Main(string[] args) {
            var sessionMgr = new SessionManager();
            var requestMgr = InitRequestManager(sessionMgr);

            var driver = new Server(requestMgr);
            driver.Start();
        }

        private static RequestManager InitRequestManager(SessionManager sessionManager) {
            var manager = new RequestManager(sessionManager);
            manager.AddHandler(new NewSessionHandler(sessionManager));
            manager.AddHandler(new FindElementHandler());
            manager.AddHandler(new ClickElementHandler());
            manager.AddHandler(new SetElementValueHandler());

            return manager;
        }

    }

}

