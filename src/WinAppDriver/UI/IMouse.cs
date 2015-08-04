namespace WinAppDriver.UI
{
    using System.Drawing;
    using System.Windows.Input;

    internal interface IMouse
    {
        Point Position { get; }

        void Click(MouseButton button);

        void DoubleClick();

        void Move(int x, int y);

        void MoveTo(int x, int y);

        void Down(MouseButton button);

        void Up(MouseButton button);
    }
}