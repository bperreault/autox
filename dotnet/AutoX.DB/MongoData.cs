#region

using System.Globalization;
using MongoDB.Bson;
using System;
using System.Xml.Linq;

#endregion

namespace AutoX.DB
{
    public class MongoData : IData
    {
        public  bool Save(XElement xElement)
        {
            if (xElement.Attribute("Created") == null)
                xElement.SetAttributeValue("Created", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            xElement.SetAttributeValue("Updated", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            return MongoDBManager.GetInstance().Save(xElement.ToBsonDocument());
        }

        public  XElement Read(string id)
        {
            return Find(id).ToXElement();
        }

        public  XElement Read(string key, string value)
        {
            return Find(key, value).ToXElement();
        }

        private static BsonDocument Find(string id)
        {
            return MongoDBManager.GetInstance().Find(id);
        }

        private static BsonDocument Find(string key, string value)
        {
            return MongoDBManager.GetInstance().Find(key, value);
        }

        public  void Delete(string id)
        {
            MongoDBManager.GetInstance().Delete(id);
        }

        public  XElement GetChildren(string parentId)
        {
            var kids = new XElement("Children");
            var direntKids = MongoDBManager.GetInstance().Kids(parentId);
            foreach (var direntKid in direntKids)
            {
                kids.Add(direntKid.ToXElement());
            }

            return kids;
        }
    }
}