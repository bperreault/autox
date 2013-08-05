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
            xElement.SetAttributeValue("Updated", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            var id = xElement.GetAttributeValue(Constants._ID);
            var parentId = xElement.GetAttributeValue(Constants.PARENT_ID);
            if (PostgreSQLDBManager.GetInstance().Find(id) == null)
            {
                xElement.SetAttributeValue("Created", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                PostgreSQLDBManager.GetInstance().CreateContent(id, xElement.ToString());
            }
            else
            {
                PostgreSQLDBManager.GetInstance().UpdateContent(id, xElement.ToString());
                PostgreSQLDBManager.GetInstance().RemoveRelationship(id);
            }
            PostgreSQLDBManager.GetInstance().CreateRelationship(parentId, "Parent-Kid", id);
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
