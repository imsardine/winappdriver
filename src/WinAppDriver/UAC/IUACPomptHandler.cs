namespace WinAppDriver.UAC
{
    internal interface IUACPomptHandler
    {
        void Activate(bool allowed);

        void Deactivate();
    }
}