namespace WinAppDriver.SystemWrapper.Windows.Input
{
    using System.Windows.Input;

    internal class KeyboardWrap : IKeyboardWrap
    {
        public KeyStates GetKeyStates(Key key)
        {
            return System.Windows.Input.Keyboard.GetKeyStates(key);
        }
    }
}