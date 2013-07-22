#region

using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    internal interface IAction
    {
        XElement Do(string data, XElement uiObj);
    }
}