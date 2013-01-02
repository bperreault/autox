// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

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