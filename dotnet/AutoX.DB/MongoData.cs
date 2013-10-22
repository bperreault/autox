#region

using System;
using System.Globalization;
using System.Xml.Linq;
using AutoX.Basic;
using MongoDB.Bson;

#endregion

namespace AutoX.DB
{
    public class MongoData : IData
    {
        public bool Save(XElement xElement)
        {
            if (xElement.Attribute("Created") == null)
                xElement.SetAttributeValue("Created", DateTime.UtcNow.ToString(Constants.DATE_TIME_FORMAT));
            xElement.SetAttributeValue("Updated", DateTime.UtcNow.ToString(Constants.DATE_TIME_FORMAT));
            return MongoDBManager.GetInstance().Save(xElement.ToBsonDocument());
        }

        public XElement Read(string id)
        {
            return Find(id).ToXElement();
        }

        public void Delete(string id)
        {
            MongoDBManager.GetInstance().Delete(id);
        }

        public XElement GetChildren(string parentId)
        {
            var kids = new XElement("Children");
            var direntKids = MongoDBManager.GetInstance().Kids(parentId);
            foreach (BsonDocument direntKid in direntKids)
            {
                kids.Add(direntKid.ToXElement());
            }

            return kids;
        }

        public XElement Read(string key, string value)
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
    }
}