// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

#endregion

#region

using System;
using System.Data.Linq.Mapping;

#endregion

namespace AutoX.Basic.Model
{
    [Table(Name = "UIObject")]
    public class UIObject : IDataObject
    {
        [Column] public string Description;
        [Column] public string Name;
        [Column] public string XPath;

        #region IDataObject Members

        [Column(IsPrimaryKey = true)]
        public string GUID { get; set; }

        [Column]
        public string EXTRA { get; set; }

        [Column]
        public DateTime Updated { get; set; }

        [Column]
        public DateTime Created { get; set; }

        #endregion
    }
}