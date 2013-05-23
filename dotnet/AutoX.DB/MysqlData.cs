using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoX.Basic;

namespace AutoX.DB
{
    public class MysqlData : IData
    {
        public bool Save(System.Xml.Linq.XElement xElement)
        {
            xElement.SetAttributeValue("Updated", DateTime.UtcNow.ToString());
            string id = xElement.GetAttributeValue("_id");
            string parentId = xElement.GetAttributeValue("_parentId");
            if (MysqlDBManager.GetInstance().Find(id) == null)
            {
                xElement.SetAttributeValue("Created", DateTime.UtcNow.ToString());
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

        public System.Xml.Linq.XElement Read(string id)
        {
            return MysqlDBManager.GetInstance().Find(id);
        }

        public void Delete(string id)
        {
            MysqlDBManager.GetInstance().Remove(id);
        }

        public System.Xml.Linq.XElement GetChildren(string id)
        {
            return MysqlDBManager.GetInstance().GetChildren(id);
        }
    }
}
