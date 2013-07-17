#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

#endregion

#region

using System;

#endregion

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

        public string Link { get; set; }

        public string InstanceId { get; set; }

        public string RunTimeId { get; set; }

        #region IDataObject Members

        public string _id { get; set; }

        public DateTime Updated { get; set; }

        public DateTime Created { get; set; }

        public string _parentId { get; set; }

        #endregion
    }

    public class StepResult : IDataObject
    {
        public string Action { get; set; }

        public string Data { get; set; }

        public string Result { get; set; }

        public string UIObject { get; set; }

        public string UIId { get; set; }

        public string Reason { get; set; }

        public string Link { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Duration { get; set; }

        public string InstanceId { get; set; }

        #region IDataObject Members

        public string _id { get; set; }

        public DateTime Updated { get; set; }

        public DateTime Created { get; set; }

        public string _parentId { get; set; }

        #endregion
    }
}