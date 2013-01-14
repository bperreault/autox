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
    public class Result : IDataObject
    {
        public string Description { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public DateTime StopTime { get; set; }

        public string Original { get; set; }

        public string Final { get; set; }

        public string Snapshot { get; set; }

        #region IDataObject Members

        public string _id { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }

        #endregion
    }
}