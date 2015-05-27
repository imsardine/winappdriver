namespace WinAppDriver.WinUserWrapper
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    internal class WinUserWrap : IWinUserWrap
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
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

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
        public bool SetForegroundWindow(IntPtr hWnd)
        {
            return WinUserExtern.SetForegroundWindow(hWnd);
        }

        private class WinUserExtern
        {
            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetMessageExtraInfo();

            [DllImport("user32.dll")]
            public static extern short VkKeyScan(char ch);

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }
    }
}