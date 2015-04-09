namespace WinAppDriver
{
    using System;
    using System.IO;

    internal class DriverContext : IDriverContext
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private string workingDirCache;

        public string GetWorkingDir()
        {
            if (this.workingDirCache == null)
            {
                string path = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\WinAppDriver");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    logger.Info("WinAppDriver working directory created: [{0}]", path);
                }

                this.workingDirCache = path;
            }

            return this.workingDirCache;
        }

        public string GetAppWorkingDir(IApplication app)
        {
            string path = Path.Combine(this.GetWorkingDir(), app.DriverAppID);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                logger.Info("Working directory for a certain app created: [{0}]; app = [{1}]", path);
            }

            return path;
        }
    }
}