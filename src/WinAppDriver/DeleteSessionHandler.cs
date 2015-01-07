using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace WinAppDriver
{
    [Route("DELETE","/session/:sessionId")]
    class DeleteSessionHandler : IHandler
    {
        private SessionManager sessionManager;

        public DeleteSessionHandler(SessionManager sessionManager) {
            this.sessionManager = sessionManager;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session) {
            var app = new StoreApplication(session.Capabilities.AppUserModelId);
            app.Terminate();
            sessionManager.DeleteSession(session.ID);
            app.Restore();

            return null;
        }
    }
}
