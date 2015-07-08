namespace WinAppDriver.UI
{
    using System.Windows.Input;

    internal interface IMouse
    {
        void Click(MouseButton button);

        void DoubleClick(MouseButton button);

        void Move(int x, int y);

        void MoveTo(int x, int y);

        void Down(MouseButton button);

        void Up(MouseButton button);
    }
}