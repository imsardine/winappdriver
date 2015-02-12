namespace WinAppDriver.WinUserWrapper
{
    using System;
    using System.Runtime.InteropServices;

    internal class WinUserWrap : IWinUserWrap
    {
        public uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize)
        {
            return WinUserExtern.SendInput(nInputs, pInputs, cbSize);
        }

        public IntPtr GetMessageExtraInfo()
        {
            return WinUserExtern.GetMessageExtraInfo();
        }

        public short VkKeyScan(char ch)
        {
            return WinUserExtern.VkKeyScan(ch);
        }

        private class WinUserExtern
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetMessageExtraInfo();

            [DllImport("user32.dll")]
            public static extern short VkKeyScan(char ch);
        }
    }
}