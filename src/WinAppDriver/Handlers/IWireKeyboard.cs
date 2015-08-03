namespace WinAppDriver.Handlers
{
    internal interface IWireKeyboard
    {
        void SendKeys(char[] keys, int delay);

        void Type(string text, int delay);
    }
}