namespace WinAppDriver.UI
{
    using System.Windows.Input;

    internal interface IKeyboard
    {
        bool IsModifierKey(Key key);

        bool IsModifierKeysPressed(ModifierKeys keys);

        void ReleaseAllModifierKeys();

        void KeyUpOrDown(Key key);

        void KeyPress(Key key);

        void Type(char key);
    }
}