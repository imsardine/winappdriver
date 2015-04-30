namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Windows.Input;
    using SystemWrapper.Windows.Input;
    using WinUserWrapper;

    internal class Keyboard : IKeyboard
    {
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1214:StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements", Justification = "Reviewed.")]
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private static readonly List<Key> ModifierKeys = new List<Key>
        {
            Key.LeftCtrl, Key.RightCtrl,
            Key.LeftAlt, Key.RightAlt,
            Key.LeftShift, Key.RightShift,
            Key.LWin, Key.RWin,
        };

        private IKeyboardWrap keyboard;

        private IWinUserWrap winUser;

        private IKeyInteropWrap keyInterop;

        public Keyboard(IKeyboardWrap keyboard, IKeyInteropWrap keyInterop, IWinUserWrap winUser)
        {
            this.keyboard = keyboard;
            this.keyInterop = keyInterop;
            this.winUser = winUser;
        }

        public bool IsModifierKey(Key key)
        {
            return ModifierKeys.Contains(key);
        }

        public bool IsModifierKeysPressed(ModifierKeys keys)
        {
            return (this.keyboard.Modifiers & keys) == keys;
        }

        public void ReleaseAllModifierKeys()
        {
            foreach (var key in ModifierKeys)
            {
                if ((this.keyboard.GetKeyStates(key) & KeyStates.Down) > 0)
                {
                    this.SendKeyboardInput(key, KEYEVENTF.KEYUP);
                }
            }
        }

        public void KeyUpOrDown(Key key)
        {
            bool down = (this.keyboard.GetKeyStates(key) & KeyStates.Down) > 0;
            logger.Debug("Toggle the (modifier) key ({0}), currently pressed? {1}", key, down);

            KEYEVENTF type = down ? KEYEVENTF.KEYUP : KEYEVENTF.KEYDOWN;
            this.SendKeyboardInput(key, type);
        }

        public void KeyPress(Key key)
        {
            this.SendKeyboardInput(key, KEYEVENTF.KEYDOWN);
            this.SendKeyboardInput(key, KEYEVENTF.KEYUP);
        }

        public void Type(char key)
        {
            short vkeyModifiers = this.winUser.VkKeyScan(key);
            if (vkeyModifiers != -1)
            {
                var vkey = vkeyModifiers & 0xff;
                var modifiers = vkeyModifiers >> 8; // high-order byte = shift state
                bool shiftNeeded = (modifiers & 1) != 0;
                bool shiftPressed = this.IsModifierKeysPressed(System.Windows.Input.ModifierKeys.Shift);

                if (shiftNeeded && !shiftPressed)
                {
                    this.SendKeyboardInput(Key.LeftShift, KEYEVENTF.KEYDOWN);
                    this.SendKeyboardInput(vkey, KEYEVENTF.KEYDOWN);
                    this.SendKeyboardInput(vkey, KEYEVENTF.KEYUP);
                    this.SendKeyboardInput(Key.LeftShift, KEYEVENTF.KEYUP);
                }
                else
                {
                    this.SendKeyboardInput(vkey, KEYEVENTF.KEYDOWN);
                    this.SendKeyboardInput(vkey, KEYEVENTF.KEYUP);
                }
            }
            else
            {
                var message = string.Format("Unicode input is not supported yet. (U+{0})", ((int)key).ToString("X"));
                throw new WinAppDriverException(message); // TODO Unicode input
            }
        }

        private void SendKeyboardInput(Key key, KEYEVENTF type)
        {
            this.SendKeyboardInput(this.keyInterop.VirtualKeyFromKey(key), type);
        }

        private void SendKeyboardInput(int vkey, KEYEVENTF type)
        {
            INPUT input = new INPUT
            {
                type = (int)INPUTTYPE.KEYBOARD,
                u = new InputUnion {
                    ki = new KEYBDINPUT
                    {
                        wVk = (ushort)vkey,
                        wScan = 0,
                        dwFlags = (uint)type,
                        dwExtraInfo = this.winUser.GetMessageExtraInfo(),
                    }
                }
            };

            this.winUser.SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}