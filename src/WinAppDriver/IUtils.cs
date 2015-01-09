namespace WinAppDriver
{
    using System;
    using System.IO;

    internal interface IUtils
    {
        string ExpandEnvironmentVariables(string input);

        void CopyDirectory(string source, string destination);
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
    }
}