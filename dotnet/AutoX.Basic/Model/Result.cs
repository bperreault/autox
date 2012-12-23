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
    [Table(Name = "Result")]
    public class Result : IDataObject
    {
        [Column]
        public string Description { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Path { get; set; }

        [Column]
        public DateTime StopTime { get; set; }

        [Column]
        public string Original { get; set; }

        [Column]
        public string Final { get; set; }

        [Column(DbType = "nvarchar(MAX)")]
        public string Snapshot { get; set; }

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