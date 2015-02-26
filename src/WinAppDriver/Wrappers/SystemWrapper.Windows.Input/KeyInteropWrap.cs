namespace WinAppDriver.SystemWrapper.Windows.Input
{
    using System.Windows.Input;

    internal class KeyInteropWrap : IKeyInteropWrap
    {
        public Key KeyFromVirtualKey(int virtualKey)
        {
            return KeyInterop.KeyFromVirtualKey(virtualKey);
        }

        public int VirtualKeyFromKey(Key key)
        {
            return KeyInterop.VirtualKeyFromKey(key);
        }
    }
}