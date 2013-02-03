#region

using MongoDB.Bson;
using System;
using System.Xml.Linq;

#endregion

namespace AutoX.DB
{
    public static class Data
    {
        public static bool Save(XElement xElement)
        {
            if (xElement.Attribute("Created") == null)
                xElement.SetAttributeValue("Created", DateTime.UtcNow.ToString());
            xElement.SetAttributeValue("Updated", DateTime.UtcNow.ToString());
            return DBManager.GetInstance().Save(xElement.ToBsonDocument());
        }

        public static XElement Read(string id)
        {
            return Find(id).ToXElement();
        }

        public static XElement Read(string key, string value)
        {
            return Find(key, value).ToXElement();
        }

        public static BsonDocument Find(string id)
        {
            return DBManager.GetInstance().Find(id);
        }

        public static BsonDocument Find(string key, string value)
        {
            return DBManager.GetInstance().Find(key, value);
        }

        public static void Delete(string id)
        {
            DBManager.GetInstance().Delete(id);
        }

        public static XElement GetChildren(string parentId)
        {
            var kids = new XElement("Children");
            var direntKids = DBManager.GetInstance().Kids(parentId);
            foreach (var direntKid in direntKids)
            {
                kids.Add(direntKid.ToXElement());
            }

            return kids;
        }
    }
}