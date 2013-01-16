#region

using System;
using System.Xml.Linq;
using MongoDB.Bson;


#endregion

namespace AutoX.DB
{
    public static class Data
    {
        public static bool Save(XElement xElement)
        {
            return DBManager.GetInstance().Save(xElement.ToBsonDocument());
        }

        public static XElement Read(string id)
        {
            return Find(id).ToXElement();
        }

        public static BsonDocument Find(string id)
        {
            return DBManager.GetInstance().Find(id);
        }

        public static void Delete(string id)
        {
            DBManager.GetInstance().Delete(id);
        }
    }
}