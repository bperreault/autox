// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Reflection;
using System.Xml.Linq;
using Newtonsoft.Json;

#endregion

namespace AutoX.Basic.Model
{
    public static class DataObjectExt
    {
        public static string GetAttributeValue(this IDataObject dataObject, string attributeName)
        {
            var field = dataObject.GetType().GetField(attributeName);
            if (field == null)
            {
                var prop = dataObject.GetType().GetProperty(attributeName);
                if (prop == null) return null;
                return prop.GetValue(dataObject, null).ToString();
            }
            return field.GetValue(dataObject).ToString();
        }

        public static void SetAttributeValue(this IDataObject dataObject, string attributeName, object value)
        {
            if (attributeName.Equals("Type") || attributeName.Equals("ParentId"))
                return;
            var field = dataObject.GetType().GetField(attributeName);
            if (field == null)
            {
                var prop = dataObject.GetType().GetProperty(attributeName);
                if (prop != null)
                {
                    prop.SetValue(dataObject, value, null);
                }
            }
            else
            {
                field.SetValue(dataObject, value);
            }
        }

        public static string GetId(this IDataObject dataObject)
        {
            return dataObject.GetAttributeValue("_id");
        }

        public static object GetObjectFromXElement(this XElement element)
        {
            var name = element.Name.ToString();
            if (!name.Contains("."))
                name = "AutoX.Basic.Model." + name;
            var type = Type.GetType(name);
            if (type != null)
            {
                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    var entity = constructor.Invoke(new Object[0]);

                    foreach (XAttribute xa in element.Attributes())
                    {
                        var prop = entity.GetType().GetProperty(xa.Name.ToString());
                        if (prop != null)
                            prop.SetValue(entity, xa.Value, null);
                    }
                    return entity;
                }
            }
            return null;
        }


        public static IDataObject JsonDeserialize(string jsonString, Type type)
        {
            return JsonConvert.DeserializeObject(jsonString, type) as IDataObject;
        }

        public static IDataObject GetDataObjectFromXElement(this XElement element)
        {
            var name = element.Name.ToString();
            var type = Type.GetType("AutoX.Basic.Model." + name);
            if (type != null)
            {
                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    var entity = constructor.Invoke(new Object[0]);
                    var ido = (IDataObject) entity;
                    foreach (XAttribute xa in element.Attributes())
                    {
                        ido.SetAttributeValue(xa.Name.ToString(), xa.Value);
                    }
                    return ido;
                }
            }
            return null;
        }

        public static string JsonSerialize(this object dataObject)
        {
            return JsonConvert.SerializeObject(dataObject);
        }


        public static XElement GetXElementFromObject(this IDataObject dataObject)
        {
            if (dataObject == null)
                return null;
            var type = dataObject.GetType();
            var tag = type.FullName;
            
            var ret = new XElement(tag);

            foreach (PropertyInfo prop in type.GetProperties())
            {
                var name = prop.Name;
                var value = (prop.GetValue(dataObject, null) ?? "") as string;
                ret.SetAttributeValue(name, value);
            }
            return ret;
        }

        public static XElement GetXElementFromDataObject(this IDataObject dataObject)
        {
            var type = dataObject.GetType();
            var tag = type.Name;
            var ret = new XElement(tag);
            ret.SetAttributeValue("Type", tag);
            foreach (FieldInfo field in type.GetFields())
            {
                var name = field.Name;
                var value = (field.GetValue(dataObject) ?? "") as string;
                ret.SetAttributeValue(name, value);
            }
            foreach (PropertyInfo prop in type.GetProperties())
            {
                var name = prop.Name;
                var value = (prop.GetValue(dataObject, null) ?? "") as string;
                ret.SetAttributeValue(name, value);
            }
            return ret;
        }
    }
}