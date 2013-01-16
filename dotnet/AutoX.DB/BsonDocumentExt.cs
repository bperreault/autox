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
            foreach (var attribure in xElement.Attributes())
            {
                var key = attribure.Name.ToString();
                var value = attribure.Value;
                bsonDocument.Add(key, value);
            }
            foreach (var kid in xElement.Descendants())
            {
                var id = kid.Attribute("_id");
                if (id == null)
                {
                    bsonDocument.Add(kid.Name.ToString(), kid.ToBsonDocument());
                }
                else
                {
                    bsonDocument.Add(id.Value, null);
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
            var dictionary = bsonDocument.ToDictionary();
            var type = dictionary.ContainsKey("_type") ? dictionary["_type"].ToString() : "Data";
            var xElement = new XElement(type);
            
            foreach (var key in dictionary.Keys)
            {
                if (usedKey.Contains(key))
                    continue;
                //if key is a guid, then refer to ...
                if (key.IsGuid())
                {
                    
                    xElement.Add(Data.Find(key).ToXElement(usedKey+"|"+key));
                    continue;
                }
                //if value is a bdoc, then it is a child
                var value = dictionary[key];
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
