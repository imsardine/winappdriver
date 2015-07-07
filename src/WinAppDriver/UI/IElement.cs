namespace WinAppDriver.UI
{
    using System.Windows;
    using System.Windows.Automation;

    internal interface IElement
    {
        string ID { get; }

        string TypeName { get; }

        string Name { get; }

        string ClassName { get; }

        bool Visible { get; }

        bool Enabled { get; }

        bool Selected { get; }

        int X { get; }

        int Y { get; }

        int Width { get; }

        int Height { get; }
    }
}