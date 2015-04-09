namespace WinAppDriver
{
    internal interface IDriverContext
    {
        string GetWorkingDir();

        string GetAppWorkingDir(IApplication app);
    }
}