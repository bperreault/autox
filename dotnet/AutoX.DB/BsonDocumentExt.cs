using AutoX.Basic;
using MongoDB.Bson;
using System;
using System.Xml.Linq;

namespace AutoX.DB
{
    public static class BsonDocumentExt
    {
        public static BsonDocument ToBsonDocument(this XElement xElement)
        {
            
            var bsonDocument = new BsonDocument();

            var type = xElement.Name.ToString();
            bsonDocument.Add(Constants._TYPE, type);
            var mainId = xElement.Attribute(Constants._ID) != null ? xElement.Attribute(Constants._ID).Value : null;

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
                var id = kid.Attribute(Constants._ID);
                if (id == null || mainId == null)
                {
                    bsonDocument.Add(kid.Name.ToString(), kid.ToBsonDocument());
                }
                else
                {
                    kid.SetAttributeValue(Constants.PARENT_ID, mainId);
                    DBFactory.GetData().Save(kid);
                }
            }
            return bsonDocument;
        }

        public static XElement ToXElement(this BsonDocument bsonDocumenet)
        {
            return bsonDocumenet.ToXElement("");
        }

        public static XElement ToXElement(this BsonDocument bsonDocument, string usedKey)
        {
            if (bsonDocument == null)
                return null;
            var dictionary = bsonDocument.ToDictionary();
            var type = dictionary.ContainsKey(Constants._TYPE) ? dictionary[Constants._TYPE].ToString() : Constants.DATA;
            var id = dictionary.ContainsKey(Constants._ID) ? dictionary[Constants._ID].ToString() : null;
            var xElement = new XElement(type);
            if (id != null)
            {
                if (usedKey.Contains(id))
                    return null;
                xElement.SetAttributeValue(Constants._ID, id);
            }

            foreach (var key in dictionary.Keys)
            {
                var value = dictionary[key];
                if (key.Equals(Constants._ID))
                    continue;
                if (usedKey.Contains(value.ToString()))
                    continue;

                //if value is a bdoc, then it is a child

                var document = value as BsonDocument;
                if (document != null)
                    xElement.Add(document.ToXElement(usedKey));
                else
                {
                    xElement.SetAttributeValue(key, value);
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