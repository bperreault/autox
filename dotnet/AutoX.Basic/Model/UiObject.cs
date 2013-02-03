// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

#endregion

#region

using System;

#endregion

namespace AutoX.Basic.Model
{
    public class UIObject : IDataObject
    {
        public string Description;
        public string Name;
        public string XPath;

        #region IDataObject Members

        public string _id { get; set; }

        public string _parentId { get; set; }

        public DateTime Updated { get; set; }

        public DateTime Created { get; set; }

        #endregion
    }
}