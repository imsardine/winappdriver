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
    //[ComImport, Guid("B1AEC16F-2383-4852-B0E9-8F0B1DC66B4D")]
    //[ComImport, Guid("F27C3930-8029-4AD1-94E3-3DBA417810C1"),
                     //InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Route("DELETE","/session/:sessionId")]
    class DeleteSessionHandler : IHandler
    {
        private SessionManager sessionManager;

        public DeleteSessionHandler(SessionManager sessionManager) {
            this.sessionManager = sessionManager;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session) {
            var app = (IStoreApplication)session.Application;

            // Exit Application 
            //Process[] processes = Process.GetProcessesByName("KKBOX.Windows");
            //processes[0].Kill();
            app.Terminate();

            // Remove Session from session manager
            sessionManager.DeleteSession(session.ID);

            // Clean content in folder
            System.Threading.Thread.Sleep(20000);
            Console.WriteLine("Delete origin and copy dest to origin begin");

            string localAppDataLocation = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%");
            string packageFamilyName = app.PackageFamilyName;
            string originSettings = localAppDataLocation + "\\Packages\\" + packageFamilyName + @"\Settings\";
            string destSettings = localAppDataLocation + "\\WinAppDriver\\InitialStates\\" + packageFamilyName + @"\Settings\";
            string originLocalState = localAppDataLocation + "\\Packages\\" + packageFamilyName + @"\LocalState\";
            string destLocalState = localAppDataLocation + "\\WinAppDriver\\InitialStates\\" + packageFamilyName + @"\LocalState\";
            Console.WriteLine("Delete Session Origin:{0}", originSettings);
            Console.WriteLine("Delete Session Dest:{0}", destSettings);

            DirectoryCopyHelper.Copy(destSettings, originSettings, true, true);
            DirectoryCopyHelper.Copy(destLocalState, originLocalState, true, true);

            return null;
        }
    }
}
