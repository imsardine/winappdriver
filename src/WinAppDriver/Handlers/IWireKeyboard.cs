namespace WinAppDriver.Handlers
{
    internal interface IWireKeyboard
    {
        void SendKeys(char[] keys);

        void Type(string text);
    }
}