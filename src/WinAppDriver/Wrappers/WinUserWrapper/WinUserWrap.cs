namespace WinAppDriver.WinUserWrapper
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
    internal class WinUserWrap : IWinUserWrap
    {
        public void mouse_event(int mouseEventFlag, int incrementX, int incrementY, int data, int extraInfo)
        {
            WinUserExtern.mouse_event(mouseEventFlag, incrementX, incrementY, data, extraInfo);
        }

        public void SetCursorPos(int x, int y)
        {
            WinUserExtern.SetCursorPos(x, y);
        }

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
            [DllImport("user32.dll")]
            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
            public static extern void mouse_event(int mouseEventFlag, int incrementX, int incrementY, int data, int extraInfo);

            [DllImport("user32.dll")]
            public static extern void SetCursorPos(int x, int y);

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