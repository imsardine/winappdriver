using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace WinAppDriver {

    [Route("POST", "/session")]
    class NewSessionHandler : IHandler {

        private SessionManager sessionManager;

        public NewSessionHandler(SessionManager sessionManager) {
            this.sessionManager = sessionManager;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session) {
            NewSessionRequest request = JsonConvert.DeserializeObject<NewSessionRequest>(body);
            foreach (var kvp in request.DesiredCapabilities)
                Console.WriteLine("{0} = {1} ({2})", kvp.Key, kvp.Value, kvp.Value.GetType());

            var caps = new Capabilities() {
                AppUserModelId = (string)request.DesiredCapabilities["appUserModelId"],
                App = (string)request.DesiredCapabilities["app"]
            };

            if (caps.App.EndsWith(".zip"))
            {
                if (caps.App.StartsWith("http"))
                {
                    caps.App = GetAppFileFromWeb(caps.App);
                }
                Console.WriteLine("\nApp file:\n\t" + caps.App);

                ZipFile.ExtractToDirectory(caps.App, caps.App.Remove(caps.App.Length - 4));
                Console.WriteLine("\nZip file extract to:\n\t" + caps.App.Remove(caps.App.Length - 4));

                UninstallApp(caps.AppUserModelId.Remove(caps.AppUserModelId.Length - 4));
                InstallApp(caps.App.Remove(caps.App.Length - 4));
            }
            else
            {
                throw new FailedCommandException("Your app file is \"" + caps.App + "\". App file is not a .zip file.", 13);
            }


            Process.Start("ActivateStoreApp", caps.AppUserModelId);
            session = sessionManager.CreateSession(caps);

            // TODO formal capabilities
            return new Dictionary<string, string>
            {
                { "platformName", "WinApp" }
            };
        }

        private class NewSessionRequest {

            [JsonProperty("desiredCapabilities")]
            internal Dictionary<string, object> DesiredCapabilities { get; set; }

            [JsonProperty("requiredCapabilities")]
            internal Dictionary<string, object> RequiredCapabilities { get; set; }

        }

        private string GetAppFileFromWeb(string webResource)
        {
            string storeFileName = Environment.GetEnvironmentVariable("TEMP") + @"\StoreApp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + webResource.Substring(webResource.LastIndexOf("."));
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();

            Console.WriteLine("Downloading File \"{0}\" .......\n\n", webResource);
            // Download the Web resource and save it into temp folder.
            myWebClient.DownloadFile(webResource, storeFileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\"", webResource);
            Console.WriteLine("\nDownloaded file saved in the following file system folder:\n\t" + storeFileName);
            return storeFileName;
        }

        private void UninstallApp(string packageFamilyName)
        {
            PowerShell ps = PowerShell.Create();
            ps.AddCommand("Get-AppxPackage");
            ps.AddParameter("Name", packageFamilyName.Remove(packageFamilyName.IndexOf("_")));
            System.Collections.ObjectModel.Collection<PSObject> package = ps.Invoke();
            if (package.Count > 0)
            {
                Console.WriteLine("\nUninstalling Windows Store App. \n");
                string packageFullName = package[0].Members["PackageFullName"].Value.ToString();
                ps = PowerShell.Create();
                ps.AddCommand("Remove-AppxPackage");
                ps.AddArgument(packageFullName);
                ps.Invoke();
            }
        }

        private void InstallApp(string fileFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(fileFolder);
            FileInfo[] files = dir.GetFiles("*.ps1", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                Console.WriteLine("\nInstalling Windows Store App. \n");
                string dirs = files[0].DirectoryName;
                PowerShell ps = PowerShell.Create();
                ps.AddScript(@"Powershell.exe -executionpolicy remotesigned -NonInteractive -File " + files[0].FullName);
                ps.Invoke();
            }
            else
            {
                throw new FailedCommandException("Cannot find .ps1 file in \"" + fileFolder + "\".", 13);
            }
        }

    }

}

