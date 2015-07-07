namespace WinAppDriver
{
    using System.Collections.Generic;

    internal interface IHandler
    {
        object Handle(Dictionary<string, string> urlParams, string json, ref ISession session);
    }
}