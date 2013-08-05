#region

using System;
using System.Globalization;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

namespace AutoX.DB
{
    public class MysqlData : IData
    {
        public bool Save(XElement xElement)
        {
            xElement.SetAttributeValue("Updated", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            var id = xElement.GetAttributeValue(Constants._ID);
            var parentId = xElement.GetAttributeValue(Constants.PARENT_ID);
            if (MysqlDBManager.GetInstance().Find(id) == null)
            {
                xElement.SetAttributeValue("Created", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                MysqlDBManager.GetInstance().CreateContent(id, xElement.ToString());
            }
            else
            {
                MysqlDBManager.GetInstance().UpdateContent(id, xElement.ToString());
                MysqlDBManager.GetInstance().RemoveRelationship(id);
            }
            MysqlDBManager.GetInstance().CreateRelationship(parentId, "Parent-Kid", id);
            return true;
        }

        public XElement Read(string id)
        {
            return MysqlDBManager.GetInstance().Find(id);
        }

        public void Delete(string id)
        {
            MysqlDBManager.GetInstance().Remove(id);
        }

        public XElement GetChildren(string id)
        {
            return MysqlDBManager.GetInstance().GetChildren(id);
        }
    }
}