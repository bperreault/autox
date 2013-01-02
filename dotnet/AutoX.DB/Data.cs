#region

using System;
using System.Xml.Linq;
using MongoDB.Bson;


#endregion

namespace AutoX.DB
{
    public static class Data
    {
        public static bool Create(XElement xElement)
        {
            throw new NotImplementedException();
        }

        public static XElement Read(string id)
        {
            throw new NotImplementedException();
        }

        public static bool Update(XElement xElement)
        {
            throw new NotImplementedException();
        }

        public static bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public static XElement BsonToXElement(this BsonDocument bsonDocument)
        {
            throw new NotImplementedException();
        }

        public static BsonDocument XElementToBson(this XElement xElement)
        {
            throw new NotImplementedException();
        }
    }
}