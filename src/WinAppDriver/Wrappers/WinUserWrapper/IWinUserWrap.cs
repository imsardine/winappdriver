namespace WinAppDriver.WinUserWrapper
{
    using System;
    using System.Runtime.InteropServices;

    internal interface IWinUserWrap
    {
        void mouse_event(int mouseEventFlag, int incrementX, int incrementY, int data, int extraInfo);

        void SetCursorPos(int x, int y);

        uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        IntPtr GetMessageExtraInfo();

        short VkKeyScan(char ch);
    }

    internal static class WinUserConstants
    {
        public const int INPUT_MOUSE = 0;

        public const int INPUT_KEYBOARD = 1;

        public const int INPUT_HARDWARE = 2; 
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct INPUT
    {
        [FieldOffset(0)]
        public int type;

        [FieldOffset(4)]
        public MOUSEINPUT mi;

        [FieldOffset(4)]
        public KEYBDINPUT ki;

        [FieldOffset(4)]
        public HARDWAREINPUT hi;
    }

    internal enum INPUTTYPE
    {
        MOUSE = 0,
        KEYBOARD = 1,
        HARDWARE = 2,
    }

    [Flags]
    internal enum KEYEVENTF : uint
    {
        KEYDOWN = 0,
        EXTENDEDKEY = 0x0001,
        KEYUP = 0x0002,
        UNICODE = 0x0004,
        SCANCODE = 0x0008,
    }

    [Flags]
    internal enum MOUSEEVENTF
    {
        MOVE = 0x0001,
        LEFTDOWN = 0x0002,
        LEFTUP = 0x0004,
        RIGHTDOWN = 0x0008,
        RIGHTUP = 0x0010,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040,
        ABSOLUTE = 0x8000,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }
}