namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using Newtonsoft.Json;

    [Route("POST", "/session/:sessionId/keys")]
    internal class SendKeysHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private static readonly Dictionary<char, Key> SpecialKeys = new Dictionary<char, Key>
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

        public SendKeysHandler(IKeyboard keyboard)
        {
            this.keyboard = keyboard;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var request = JsonConvert.DeserializeObject<ElementValueRequest>(body);
            var keys = string.Join(string.Empty, request.KeySequence);
            logger.Debug("Keys: {0}", keys);

            foreach (var keyChar in keys)
            {
                if (keyChar >= '\uE000' && keyChar <= '\uF8FF') // Private Use Area
                {
                    logger.Debug("Special key: U+{0}", ((int)keyChar).ToString("X"));
                    Key key = SendKeysHandler.SpecialKeys[keyChar];
                    if (keyboard.IsModifierKey(key))
                    {
                        keyboard.KeyUpOrDown(key);
                    }
                    else
                    {
                        keyboard.KeyPress(key);
                    }
                }
                else
                {
                    logger.Debug("General key/char: {0}", keyChar);
                    keyboard.Type(keyChar);
                }
            }

            return null;
        }

        private class ElementValueRequest
        {
            [JsonProperty("value")]
            internal string[] KeySequence { get; set; }
        }
    }
}