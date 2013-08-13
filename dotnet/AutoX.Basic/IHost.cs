#region

// Hapa Project, CC
// Created @2012 08 29 08:09
// Last Updated  by Huang, Jien @2012 08 29 08:09

#region

using System.Xml.Linq;

#endregion

#endregion

namespace AutoX.Basic
{
    public interface IHost
    {
        XElement GetDataObject(string id);

        Config GetConfig();

        void SetCommand(XElement steps);

        void Stop();

        XElement GetResult();
        
    }
}