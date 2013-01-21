using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MongoDB.Bson;

namespace AutoX.DB
{
    public static class BsonDocumentExt
    {
        public static BsonDocument ToBsonDocument(this XElement xElement)
        {
            var bsonDocument = new BsonDocument();
            
            var type = xElement.Name.ToString();
            bsonDocument.Add("_type", type);
            var mainId = xElement.Attribute("_id")!=null? xElement.Attribute("_id").Value:null;

            foreach (var attribure in xElement.Attributes())
            {
                var key = attribure.Name.ToString();
                var value = attribure.Value;
                if (bsonDocument.Contains(key))
                    bsonDocument.Set(key, value);
                else
                    bsonDocument.Add(key, value);
            }
            foreach (var kid in xElement.Descendants())
            {
                var id = kid.Attribute("_id");
                if (id == null||mainId==null)
                {
                    bsonDocument.Add(kid.Name.ToString(), kid.ToBsonDocument());
                }
                else
                {
                    kid.SetAttributeValue("_parentId",mainId);
                    Data.Save(kid);
                }
            }
            return bsonDocument;
        }

        public static XElement ToXElement(this BsonDocument bsonDocumenet)
        {
            return bsonDocumenet.ToXElement("");
        }
        public static XElement ToXElement(this BsonDocument bsonDocument,string usedKey)
        {
            if (bsonDocument == null)
                return null;
            var dictionary = bsonDocument.ToDictionary();
            var type = dictionary.ContainsKey("_type") ? dictionary["_type"].ToString() : "Data";
            var id = dictionary.ContainsKey("_id") ? dictionary["_id"].ToString() : null;
            var xElement = new XElement(type);
            if (id != null)
            {
                if (usedKey.Contains(id))
                    return null;
                xElement.SetAttributeValue("_id", id);
            }


            foreach (var key in dictionary.Keys)
            {
                var value = dictionary[key];
                if (key.Equals("_id"))
                    continue;
                if (usedKey.Contains(value.ToString()))
                    continue;
                
                //if value is a bdoc, then it is a child
                
                var document = value as BsonDocument;
                if(document != null)
                    xElement.Add(document.ToXElement(usedKey));
                else
                {
                    xElement.SetAttributeValue(key,value);
                }
            }
            return xElement;
        }

        public static bool IsGuid(this string id)
        {
            try
            {
                new Guid(id);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    
}
