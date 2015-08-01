namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using WinAppDriver.UI;

    internal class WireKeyboard : IWireKeyboard
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private readonly Dictionary<char, Key> specialKeys = new Dictionary<char, Key>
        {
            { '\uE000', Key.None },
            { '\uE001', Key.Cancel },
            { '\uE002', Key.Help },
            { '\uE003', Key.Back },
            { '\uE004', Key.Tab },
            { '\uE005', Key.Clear },
            { '\uE006', Key.Return },
            { '\uE007', Key.Enter },
            { '\uE008', Key.LeftShift },
            { '\uE009', Key.LeftCtrl },
            { '\uE00A', Key.LeftAlt },
            { '\uE00B', Key.Pause },
            { '\uE00C', Key.Escape },
            { '\uE00D', Key.Space },
            { '\uE00E', Key.PageUp },
            { '\uE00F', Key.PageDown },
            { '\uE010', Key.End },
            { '\uE011', Key.Home },
            { '\uE012', Key.Left },
            { '\uE013', Key.Up },
            { '\uE014', Key.Right },
            { '\uE015', Key.Down },
            { '\uE016', Key.Insert },
            { '\uE017', Key.Delete },
            { '\uE018', Key.None }, // TODO Semicolon ?
            { '\uE019', Key.None }, // TODO Equals ?
            { '\uE01A', Key.NumPad0 },
            { '\uE01B', Key.NumPad1 },
            { '\uE01C', Key.NumPad2 },
            { '\uE01D', Key.NumPad3 },
            { '\uE01E', Key.NumPad4 },
            { '\uE01F', Key.NumPad5 },
            { '\uE020', Key.NumPad6 },
            { '\uE021', Key.NumPad7 },
            { '\uE022', Key.NumPad8 },
            { '\uE023', Key.NumPad9 },
            { '\uE024', Key.Multiply },
            { '\uE025', Key.Add },
            { '\uE026', Key.Separator },
            { '\uE027', Key.Subtract },
            { '\uE028', Key.Decimal },
            { '\uE029', Key.Divide },
            { '\uE031', Key.F1 },
            { '\uE032', Key.F2 },
            { '\uE033', Key.F3 },
            { '\uE034', Key.F4 },
            { '\uE035', Key.F5 },
            { '\uE036', Key.F6 },
            { '\uE037', Key.F7 },
            { '\uE038', Key.F8 },
            { '\uE039', Key.F9 },
            { '\uE03A', Key.F10 },
            { '\uE03B', Key.F11 },
            { '\uE03C', Key.F12 },
            { '\uE03D', Key.LWin },
        };

        private IKeyboard keyboard;

        public WireKeyboard(IKeyboard keyboard)
        {
            this.keyboard = keyboard;
        }

        public void SendKeys(char[] keys)
        {
            this.SendKeysImpl(keys);
        }

        public void Type(string text)
        {
            this.SendKeysImpl(text.ToCharArray());
            this.keyboard.ReleaseAllModifierKeys();
        }

        private void SendKeysImpl(char[] keys)
        {
            foreach (var keyChar in keys)
            {
                if (keyChar >= '\uE000' && keyChar <= '\uF8FF')
                {
                    // Private Use Area
                    logger.Debug("Special key: U+{0}", ((int)keyChar).ToString("X"));
                    Key key = this.specialKeys[keyChar];
                    if (this.keyboard.IsModifierKey(key))
                    {
                        this.keyboard.KeyUpOrDown(key);
                    }
                    else
                    {
                        this.keyboard.KeyPress(key);
                    }
                }
                else
                {
                    logger.Debug("General key/char: {0}", keyChar);
                    this.keyboard.Type(keyChar);
                }
            }
        }
    }
}