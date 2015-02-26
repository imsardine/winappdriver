namespace WinAppDriver.SystemWrapper.Windows.Input
{
    using System.Windows.Input;

    internal class KeyboardWrap : IKeyboardWrap
    {
        public KeyStates GetKeyStates(Key key)
        {
            return Keyboard.GetKeyStates(key);
        }

        public ModifierKeys Modifiers
        {
            get
            {
                return Keyboard.Modifiers;
            }
        }
    }
}