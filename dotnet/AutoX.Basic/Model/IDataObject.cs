// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;

#endregion

namespace AutoX.Basic.Model
{
    public interface IDataObject
    {
        //just a mark
        //string Name { get; set; }
        string _id { get; set; }
        string _parentId { get; set; }
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }
}