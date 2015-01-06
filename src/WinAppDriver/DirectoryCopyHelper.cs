using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WinAppDriver
{
    class DirectoryCopyHelper
    {
        public static void Copy(string sourcePath, string destinationPath, bool recursive, bool preservePermissions)
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

                if(Directory.Exists(destinationPath))
                {
                    
                        Console.WriteLine("DCH: Directory Delete Begin");
                        Directory.Delete(destinationPath, true);
                        Console.WriteLine("DCH: Directory Delete Complete");
                   
                        //Console.WriteLine("DCH: Caught Exception");
             
                }

                if (!Directory.Exists(destinationPath))
                {
                    Console.WriteLine("DCH: Directory Create Begin");
                    Directory.CreateDirectory(destinationPath);
                    Console.WriteLine("DCH: Directory Create Complete");
                }

                // Copy directory security
                if (preservePermissions)
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
                    if (preservePermissions)
                    {
                        var security = sourceFile.GetAccessControl();
                        security.SetAccessRuleProtection(true, true);
                        File.SetAccessControl(destinationFilePath, security);
                    }

                }

                if (recursive)
                {
                    foreach (DirectoryInfo soiurceSubDirectory in sourceSubFolders)
                    {
                        string temppath = Path.Combine(destinationPath, soiurceSubDirectory.Name);
                        Copy(soiurceSubDirectory.FullName, temppath, recursive, preservePermissions);
                    }
                }
            }
        }
    }
}
