namespace WinAppDriver.SystemWrapper.Windows.Input
{
    using System.Windows.Input;

    internal interface IKeyboardWrap
    {
        ModifierKeys Modifiers { get; }

        KeyStates GetKeyStates(Key key);
    }
}
