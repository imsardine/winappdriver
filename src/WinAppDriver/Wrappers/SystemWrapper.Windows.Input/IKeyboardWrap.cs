namespace WinAppDriver.SystemWrapper.Windows.Input
{
    using System.Windows.Input;

    internal interface IKeyboardWrap
    {
        KeyStates GetKeyStates(Key key);
    }
}
