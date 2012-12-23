// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Data.Linq.Mapping;

#endregion

namespace AutoX.Basic.Model
{
    [Table(Name = "Translation")]
    public class Translation : IDataObject
    {
        #region IDataObject Members

        [Column(IsPrimaryKey = true)]
        public string GUID { get; set; }

        [Column(DbType = "NVarChar(MAX)")]
        public string EXTRA { get; set; }

        //put all data into this field

        [Column]
        public DateTime Updated { get; set; }

        [Column]
        public DateTime Created { get; set; }

        #endregion
    }
}