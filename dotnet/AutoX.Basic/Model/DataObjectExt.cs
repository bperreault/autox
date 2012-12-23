// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Reflection;
using System.Xml.Linq;

#endregion

namespace AutoX.Basic.Model
{
    public static class DataObjectExt
    {
        public static string GetAttributeValue(this IDataObject dataObject, string attributeName)
        {
            FieldInfo field = dataObject.GetType().GetField(attributeName);
            if (field == null)
            {
                PropertyInfo prop = dataObject.GetType().GetProperty(attributeName);
                if (prop == null)
                {
                    string extra = dataObject.EXTRA;
                    return extra == null ? null : XElement.Parse(extra).GetAttributeValue(attributeName);
                }
                return prop.GetValue(dataObject, null).ToString();
            }
            return field.GetValue(dataObject).ToString();
        }

        public static void SetAttributeValue(this IDataObject dataObject, string attributeName, object value)
        {
            if (attributeName.Equals("Type") || attributeName.Equals("ParentId"))
                return;
            FieldInfo field = dataObject.GetType().GetField(attributeName);
            if (field == null)
            {
                PropertyInfo prop = dataObject.GetType().GetProperty(attributeName);
                if (prop == null)
                {
                    string extra = dataObject.EXTRA;
                    if (extra == null)
                    {
                        var xExtra = new XElement("Extra");
                        xExtra.SetAttributeValue(attributeName, value);
                        dataObject.EXTRA = xExtra.ToString();
                    }
                    else
                    {
                        XElement xExtra = XElement.Parse(extra);
                        xExtra.SetAttributeValue(attributeName, value);
                        dataObject.EXTRA = xExtra.ToString();
                    }
                }
                else
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
            return dataObject.GetAttributeValue("GUID");
        }

        public static object GetObjectFromXElement(this XElement element)
        {
            string name = element.Name.ToString();
            if (!name.Contains("."))
                name = "AutoX.Basic.Model." + name;
            Type type = Type.GetType(name);
            if (type != null)
            {
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    object entity = constructor.Invoke(new Object[0]);

                    foreach (XAttribute xa in element.Attributes())
                    {
                        PropertyInfo prop = entity.GetType().GetProperty(xa.Name.ToString());
                        if (prop != null)
                            prop.SetValue(entity, xa.Value, null);
                    }
                    return entity;
                }
            }
            return null;
        }

        public static IDataObject GetDataObjectFromXElement(this XElement element)
        {
            string name = element.Name.ToString();
            Type type = Type.GetType("AutoX.Basic.Model." + name);
            if (type != null)
            {
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    object entity = constructor.Invoke(new Object[0]);
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

        public static XElement GetXElementFromObject(this object dataObject)
        {
            if (dataObject == null)
                return null;
            Type type = dataObject.GetType();
            string tag = type.FullName;
            if (tag == null)
            {
                return null;
            }
            var ret = new XElement(tag);

            foreach (PropertyInfo prop in type.GetProperties())
            {
                string name = prop.Name;
                var value = (prop.GetValue(dataObject, null) ?? "") as string;
                ret.SetAttributeValue(name, value);
            }
            return ret;
        }

        public static XElement GetXElementFromDataObject(this IDataObject dataObject)
        {
            Type type = dataObject.GetType();
            string tag = type.Name;
            var ret = new XElement(tag);
            ret.SetAttributeValue("Type", tag);
            foreach (FieldInfo field in type.GetFields())
            {
                string name = field.Name;
                var value = (field.GetValue(dataObject) ?? "") as string;
                ret.SetAttributeValue(name, value);
            }
            foreach (PropertyInfo prop in type.GetProperties())
            {
                string name = prop.Name;
                var value = (prop.GetValue(dataObject, null) ?? "") as string;
                if (name.Equals("Extra"))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        foreach (XAttribute attr in XElement.Parse(value).Attributes())
                        {
                            ret.SetAttributeValue(attr.Name, attr.Value);
                        }
                    }
                }
                else
                    ret.SetAttributeValue(name, value);
            }
            return ret;
        }
    }
}