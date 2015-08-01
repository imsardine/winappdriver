namespace WinAppDriver.UI
{
    using System.Collections.Generic;
    using System.Drawing;

    internal interface IOverlay
    {
        IElement ContextElement { set; }

        IElement HighlightedElement { set; }

        IList<IElement> HighlightedElements { set; }

        Point? PointTo { set; }

        void Open();

        void Clear();

        void Show();
    }
}