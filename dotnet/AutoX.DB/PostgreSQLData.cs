using System;
using System.Globalization;
using System.Xml.Linq;
using AutoX.Basic;

namespace AutoX.DB
{
    public class PostgreSQLData : IData
    {
        public bool Save(XElement xElement)
        {
            xElement.SetAttributeValue("Updated", DateTime.UtcNow.ToString(Constants.DATE_TIME_FORMAT));
            var id = xElement.GetAttributeValue(Constants._ID);
            var parentId = xElement.GetAttributeValue(Constants.PARENT_ID);
            var updated = xElement.GetAttributeValue("Updated");
            var created = DateTime.UtcNow.ToString(Constants.DATE_TIME_FORMAT);
            if (PostgreSQLDBManager.GetInstance().Find(id) == null)
            {
                xElement.SetAttributeValue("Created", created);
                var _type = xElement.GetAttributeValue("_type");
                
                PostgreSQLDBManager.GetInstance().CreateContent(id, xElement.ToString(),_type,created,updated);
            }
            else
            {
                var existed_created = xElement.GetAttributeValue("Created");
                DateTime eDateTime = DateTime.Parse(existed_created);
                if (!string.IsNullOrEmpty(existed_created)) 
                    created = eDateTime.ToString(Constants.DATE_TIME_FORMAT);
                PostgreSQLDBManager.GetInstance().UpdateContent(id, xElement.ToString(),updated);
                PostgreSQLDBManager.GetInstance().RemoveRelationship(id);
            }
            PostgreSQLDBManager.GetInstance().CreateRelationship(parentId, "Parent-Kid", id,created,updated);
            return true;
        }

        public XElement Read(string id)
        {
            return PostgreSQLDBManager.GetInstance().Find(id);
        }

        public void Delete(string id)
        {
            PostgreSQLDBManager.GetInstance().Remove(id);
        }

        public XElement GetChildren(string id)
        {
            return PostgreSQLDBManager.GetInstance().GetChildren(id);
        }
    }
}
