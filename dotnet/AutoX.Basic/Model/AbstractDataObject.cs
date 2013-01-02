#region

using System;

#endregion

namespace AutoX.Basic.Model
{
    public class AbstractDataObject : IDataObject
    {
        public string GUID
        {
            get { throw new NotImplementedException(); }
            set { ; }
        }

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
    }
}