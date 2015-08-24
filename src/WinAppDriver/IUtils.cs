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

        bool DirectoryExists(string path);

        void CopyDirectoryAndSecurity(string src, string dir);

        void CopyFileAndSecurity(string src, string dir);

        bool CreateDirectoryIfNotExists(string path);

        bool DeleteDirectoryIfExists(string path);

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

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CopyDirectoryAndSecurity(string src, string dir)
        {
            var dest = Path.Combine(dir, Path.GetFileName(src));
            logger.Debug("Copy directory (and security) from \"{0}\" to \"{1}\".", src, dest);

            this.DeleteDirectoryIfExists(dest);
            this.CreateDirectoryIfNotExists(dest);
            {
                var security = Directory.GetAccessControl(src);
                security.SetAccessRuleProtection(true, true);
                Directory.SetAccessControl(dest, security);
            }

            foreach (var file in Directory.GetFiles(src))
            {
                this.CopyFileAndSecurity(file, dir);
            }

            foreach (var subdir in Directory.GetDirectories(src))
            {
                this.CopyDirectoryAndSecurity(subdir, dest);
            }
        }

        public void CopyFileAndSecurity(string src, string dir)
        {
            var dest = Path.Combine(dir, Path.GetFileName(src));
            logger.Debug("Copy file (and security) from \"{0}\" to \"{1}\".", src, dest);

            File.Copy(src, dest, true);

            var security = File.GetAccessControl(src);
            security.SetAccessRuleProtection(true, true);
            File.SetAccessControl(dest, security);
        }

        public bool CreateDirectoryIfNotExists(string path)
        {
            if (this.DirectoryExists(path))
            {
                return false;
            }
            else
            {
                Directory.CreateDirectory(path);
                return true;
            }
        }

        public bool DeleteDirectoryIfExists(string path)
        {
            if (this.DirectoryExists(path))
            {
                foreach (var subdir in Directory.GetDirectories(path))
                {
                    this.DeleteDirectoryIfExists(subdir);
                }

                foreach (var file in Directory.GetFiles(path))
                {
                    logger.Debug("Delete file: {0}", file);
                    File.Delete(file);
                }

                logger.Debug("Delete directory: {0}", path);

                try
                {
                    Directory.Delete(path);
                }
                catch (IOException e)
                {
                    // Seems like a timing issue that the built-in Directory.Delete(path, true)
                    // did not take into account.
                    logger.Warn("IOException raised while deleting the directory: {0} ({1})", path, e.Message);
                    logger.Debug("Sleep for a while (5s), and try again...");

                    System.Threading.Thread.Sleep(5000);
                    Directory.Delete(path);
                }

                return true;
            }
            else
            {
                return false;
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