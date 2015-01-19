namespace WinAppDriver
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;

    internal interface IUtils
    {
        string ExpandEnvironmentVariables(string input);

        void CopyDirectory(string source, string destination);

        string GetFileMD5(string filePath);

        string GetAppFileFromWeb(string webResource, string expectFileMD5);
    }

    internal class Utils : IUtils
    {
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

                Console.WriteLine("DCH: Directory Copy Begin");
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

        public string GetFileMD5(string filePath)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            FileStream zipFile = new FileStream(filePath, FileMode.Open);
            byte[] bytes = md5.ComputeHash(zipFile);
            zipFile.Close();
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
        }

        public string GetAppFileFromWeb(string webResource, string expectFileMD5)
        {
            string storeFileName = Environment.GetEnvironmentVariable("TEMP") + @"\StoreApp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + webResource.Substring(webResource.LastIndexOf("."));

            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();

            Console.WriteLine("Downloading File \"{0}\" .......\n\n", webResource);

            // Download the Web resource and save it into temp folder.
            myWebClient.DownloadFile(webResource, storeFileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\"", webResource);
            Console.WriteLine("\nDownloaded file saved in the following file system folder:\n\t" + storeFileName);

            string fileMD5 = this.GetFileMD5(storeFileName);
            if (expectFileMD5 != null && expectFileMD5 != this.GetFileMD5(storeFileName))
            {
                string msg = "You got a wrong file. ExpectMD5 is \"" + expectFileMD5 + "\", but download file MD5 is \"" + fileMD5 + "\".";
                throw new WinAppDriverException(msg);
            }

            return storeFileName;
        }
    }
}