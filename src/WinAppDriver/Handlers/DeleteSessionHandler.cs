namespace WinAppDriver.Handlers
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

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var app = session.Application;
            this.sessionManager.DeleteSession(session.ID);

            var caps = session.Capabilities;
            if (caps.ResetStrategy == ResetStrategy.Fast)
            {
                app.Terminate();
                app.RestoreInitialStates();
            }
            else if (caps.ResetStrategy == ResetStrategy.Full)
            {
                app.Terminate();
                app.Uninstall();
            }

            return null;
        }
    }
}