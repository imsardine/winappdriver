namespace WinAppDriver
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;

    internal interface IUtils
    {
        string ExpandEnvironmentVariables(string input);

        void CopyDirectory(string source, string destination);

        void DeleteDirectory(string source);

        string GetFileMD5(string filePath);

        string GetAppFileFromWeb(string webResource, string checksum);

        Process Execute(string command, IDictionary<string, string> envs);
    }

    internal class Utils : IUtils
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        public string ExpandEnvironmentVariables(string input)
        {
            return Environment.ExpandEnvironmentVariables(input);
        }

        public void CopyDirectory(string sourcePath, string destinationPath)
        {
            if (Directory.Exists(sourcePath))
            {
                DirectoryInfo sourceInfo = new DirectoryInfo(sourcePath);
                DirectoryInfo destinationInfo = new DirectoryInfo(destinationPath);
                DirectoryInfo[] sourceSubFolders = sourceInfo.GetDirectories();
                FileInfo[] sourceFiles = sourceInfo.GetFiles();

                if (!sourceInfo.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourcePath);
                }

                if (Directory.Exists(destinationPath))
                {
                    Directory.Delete(destinationPath, true);
                }

                Directory.CreateDirectory(destinationPath);

                // Copy directory security
                {
                    var security = sourceInfo.GetAccessControl();
                    security.SetAccessRuleProtection(true, true);
                    destinationInfo.SetAccessControl(security);
                }

                logger.Debug("DCH: Directory Copy Begin");
                foreach (FileInfo sourceFile in sourceFiles)
                {
                    string destinationFilePath = Path.Combine(destinationPath, sourceFile.Name);
                    sourceFile.CopyTo(destinationFilePath, false);

                    // Copy file security
                    var security = sourceFile.GetAccessControl();
                    security.SetAccessRuleProtection(true, true);
                    File.SetAccessControl(destinationFilePath, security);
                }

                foreach (DirectoryInfo sourceSubFolder in sourceSubFolders)
                {
                    string temppath = Path.Combine(destinationPath, sourceSubFolder.Name);
                    this.CopyDirectory(sourceSubFolder.FullName, temppath);
                }
            }
        }

        public void DeleteDirectory(string sourcePath)
        {
            if (Directory.Exists(sourcePath))
            {
                DirectoryInfo sourceInfo = new DirectoryInfo(sourcePath);
                sourceInfo.Delete(true);
            }
        }

        public string GetFileMD5(string filePath)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            FileStream zipFile = new FileStream(filePath, FileMode.Open);
            byte[] bytes = md5.ComputeHash(zipFile);
            zipFile.Close();
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
        }

        public string GetAppFileFromWeb(string webResource, string checksum)
        {
            string storeFileName = Environment.GetEnvironmentVariable("TEMP") + @"\StoreApp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + webResource.Substring(webResource.LastIndexOf("."));

            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();

            logger.Info("Downloading file \"{0}\" .......", webResource);
            int retryCounter = 3;
            while (retryCounter > 0)
            {
                retryCounter--;
                
                // Download the Web resource and save it into temp folder.
                myWebClient.DownloadFile(webResource, storeFileName);
 
                string checksumActual = this.GetFileMD5(storeFileName);
                if (checksum != null && checksum != this.GetFileMD5(storeFileName))
                {
                    if (retryCounter == 0)
                    {
                        string msg = "You got a wrong file. Expected checksum is \"" + checksum + "\", but downloaded file checksum is \"" + checksumActual + "\".";
                        throw new WinAppDriverException(msg);
                    }
                    else
                    {
                        logger.Debug("Expected checksum is \"{0}\", but downloaded file checksum is \"{1}\".", checksum, checksumActual);
                        logger.Debug("Retry downloading file \"{0}\" .......", webResource);
                    }
                }
                else
                {
                    logger.Debug("Successfully downloaded file from \"{0}\"", webResource);
                    logger.Debug("Downloaded file saved in the following file system folder: \"{0}\"", storeFileName);
                    break;
                }
            }

            return storeFileName;
        }

        public Process Execute(string command, IDictionary<string, string> variables)
        {
            string executable, arguments;
            this.ParseCommand(command, out executable, out arguments);

            // TODO not exited, or non-zero exit status
            var startInfo = new ProcessStartInfo(executable, arguments);
            startInfo.UseShellExecute = false; // mandatory for setting environment variables
            if (variables != null)
            {
                var envs = startInfo.EnvironmentVariables;
                foreach (var item in variables)
                {
                    envs[item.Key] = item.Value;
                }
            }

            return Process.Start(startInfo);
        }

        private void ParseCommand(string command, out string executable, out string arguments)
        {
            // TODO quoted executable
            int index = command.IndexOf(' '); // the first space as the separator
            if (index == -1)
            {
                executable = command;
                arguments = null;
            }
            else
            {
                executable = command.Substring(0, index);
                arguments = command.Substring(index);
            }

            // TODO PowerShell scripts -> use PowerShell.exe and append '-ExecutionPolicy Bypass' (if not provided)
            logger.Debug(
                "Parse command; [{0}] ==> executable = [{1}], arguments = [{2}]",
                command,
                executable,
                arguments);
        }
    }
}