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
    [Table(Name = "Script")]
    public class Script : IDataObject
    {
        [Column(DbType = "nvarchar(MAX)")] public string Content;
        [Column] public string Description;
        [Column] public string Name;
        [Column] public string ScriptType;

        #region IDataObject Members

        [Column(IsPrimaryKey = true)]
        public string _id { get; set; }

        [Column]
        public string EXTRA { get; set; }

        [Column]
        public DateTime Updated { get; set; }

        [Column]
        public DateTime Created { get; set; }

        #endregion
    }
}