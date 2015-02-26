namespace WinAppDriver.SystemWrapper.Windows.Input
{
    using System.Windows.Input;

    internal interface IKeyInteropWrap
    {
        Key KeyFromVirtualKey(int virtualKey);

        int VirtualKeyFromKey(Key key);
    }
}