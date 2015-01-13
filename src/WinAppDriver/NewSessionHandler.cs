namespace WinAppDriver
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Management.Automation;
    using System.Net;
    using Newtonsoft.Json;

    [Route("POST", "/session")]
    internal class NewSessionHandler : IHandler
    {
        private SessionManager sessionManager;

        private IUtils utils;

        public NewSessionHandler(SessionManager sessionManager, IUtils utils)
        {
            this.sessionManager = sessionManager;
            this.utils = utils;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            NewSessionRequest request = JsonConvert.DeserializeObject<NewSessionRequest>(body);
            foreach (var kvp in request.DesiredCapabilities)
            {
                Console.WriteLine("{0} = {1} ({2})", kvp.Key, kvp.Value, kvp.Value.GetType());
            }

            var caps = new Capabilities()
            {
                PlatformName = (string)request.DesiredCapabilities["platformName"],
                AppUserModelId = (string)request.DesiredCapabilities["appUserModelId"],
                App = (string)request.DesiredCapabilities["app"]
            };

            IStoreApplication app = new StoreApplication(caps.AppUserModelId, this.utils);
            if (caps.App.EndsWith(".zip"))
            {
                if (caps.App.StartsWith("http"))
                {
                    caps.App = this.GetAppFileFromWeb(caps.App);
                }

                Console.WriteLine("\nApp file:\n\t" + caps.App);

                if (app.IsInstalled())
                {
                    this.UninstallApp(app.GetPackageFullName());
                }

                this.InstallApp(caps.App);
            }
            else
            {
                throw new FailedCommandException("Your app file is \"" + caps.App + "\". App file is not a .zip file.", 13);
            }

            app.BackupInitialStates(); // TODO only when newly installed
            app.Activate();
            session = this.sessionManager.CreateSession(app, caps);

            return caps; // TODO capabilities
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

        private void UninstallApp(string packageFullName)
        {
            PowerShell ps = PowerShell.Create();
            ps.AddCommand("Remove-AppxPackage");
            ps.AddArgument(packageFullName);
            ps.Invoke();
        }

        private void InstallApp(string zipFile)
        {
            string fileFolder = zipFile.Remove(zipFile.Length - 4);
            ZipFile.ExtractToDirectory(zipFile, fileFolder);
            Console.WriteLine("\nZip file extract to:\n\t" + fileFolder);

            DirectoryInfo dir = new DirectoryInfo(fileFolder);
            FileInfo[] files = dir.GetFiles("*.ps1", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                Console.WriteLine("\nInstalling Windows Store App. \n");
                string dirs = files[0].DirectoryName;
                PowerShell ps = PowerShell.Create();
                ps.AddScript(@"Powershell.exe -executionpolicy remotesigned -NonInteractive -File " + files[0].FullName);
                ps.Invoke();
                System.Threading.Thread.Sleep(1000); // Waiting activity done.
            }
            else
            {
                throw new FailedCommandException("Cannot find .ps1 file in \"" + fileFolder + "\".", 13);
            }
        }

        private class NewSessionRequest
        {
            [JsonProperty("desiredCapabilities")]
            internal Dictionary<string, object> DesiredCapabilities { get; set; }

            [JsonProperty("requiredCapabilities")]
            internal Dictionary<string, object> RequiredCapabilities { get; set; }
        }
    }
}
