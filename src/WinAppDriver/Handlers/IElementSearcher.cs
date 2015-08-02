namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    internal interface IElementSearcher
    {
        IElement FindFirst(IElement context, LocatorStrategy strategy, string locator);

        IList<IElement> FindAll(IElement context, LocatorStrategy strategy, string locator);
    }
}