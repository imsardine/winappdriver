using System.Collections.Generic;

namespace WinAppDriver {

    interface IHandler {

        object Handle(Dictionary<string, string> urlParams, string json, Session session);

    }

}

