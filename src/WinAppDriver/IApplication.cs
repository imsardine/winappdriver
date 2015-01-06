namespace WinAppDriver
{
    interface IApplication
    {
        bool IsInstalled();

        void Activate();

        void Terminate();

        void Reset();
    }
}
