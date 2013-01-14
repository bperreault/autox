// Hapa Project, CC
// Created @2012 08 29 08:09
// Last Updated  by Huang, Jien @2012 08 29 08:09

#region

using System.Xml.Linq;
using AutoX.Basic.Model;

#endregion

namespace AutoX.Basic
{
    public interface IHost
    {
        XElement GetDataObject(string id);

        void SetCommand( XElement steps);

        XElement GetResult( string guid);
    }
}