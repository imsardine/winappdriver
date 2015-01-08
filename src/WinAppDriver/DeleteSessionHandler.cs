namespace WinAppDriver
{
    using System.Collections.Generic;

    [Route("DELETE", "/session/:sessionId")]
    internal class DeleteSessionHandler : IHandler
    {
        private SessionManager sessionManager;

        public DeleteSessionHandler(SessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var app = (IStoreApplication)session.Application;
            this.sessionManager.DeleteSession(session.ID);
            app.Terminate();
            app.RestoreInitialStates();

            return null;
        }
    }
}