// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Xml.Linq;

#endregion

namespace AutoX.Basic.Model
{
    public class Instance : IDataObject
    {
        public string TestName { get; set; }
        public string SuiteName { get; set; }
        public string Status { get; set; }
        public string ClientName { get; set; }
        public string Language { get; set; }
        public string ScriptGUID { get; set; }
        public string GUID { get; set; }


        public string EXTRA
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public DateTime Created
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public DateTime Updated
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public XElement ToXElement()
        {
            return this.GetXElementFromObject();
        }

        public static Instance FromXElement(XElement element)
        {
            return element.GetObjectFromXElement() as Instance;
        }
    }
}