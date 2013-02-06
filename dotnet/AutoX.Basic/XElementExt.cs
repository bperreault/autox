// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#endregion

namespace AutoX.Basic
{
    public static class XElementExt
    {
        public static void DealExtra(this XElement rElement)
        {
            var xAttribute = rElement.Attribute("EXTRA");
            if (xAttribute == null) return;
            var eData = xAttribute.Value;
            if (!string.IsNullOrEmpty(eData))
            {
                var xExtra = XElement.Parse(eData);
                foreach (XAttribute attribute in xExtra.Attributes())
                {
                    rElement.SetAttributeValue(attribute.Name, attribute.Value);
                }
            }
            xAttribute.Remove();
        }

        public static XElement GetRootPartElement(this XElement x)
        {
            var nElement = new XElement(x.Name);
            foreach (XAttribute xattr in x.Attributes())
            {
                nElement.SetAttributeValue(xattr.Name, xattr.Value);
            }
            return nElement;
        }

        public static string GenerateXPathFromXElement(this XElement xUi)
        {
            var original = xUi.GetAttributeValue("XPath");
            if (!string.IsNullOrEmpty(original))
                return original;
            var tag = "//*";
            var xTag = xUi.Attribute("tag");
            if (xTag != null)
            {
                tag = "//" + xTag.Value;
                xTag.Remove();
            }
            var xpath = tag;
            if (xUi.Attributes().Any())
            {
                xpath = xpath + "[";
                var count = 0;
                foreach (XAttribute xa in xUi.Attributes())
                {
                    var key = xa.Name.ToString();
                    if (key.StartsWith("_"))
                        continue;
                    var value = xa.Value;
                    if (count > 0)
                        xpath = xpath + " and ";
                    if (key.Equals("text"))
                    {
                        if (value.Length < 32)
                            xpath = xpath + key + "()='" + value + "' ";
                        else
                        {
                            xpath = xpath + "contains(text(),'" + value.Substring(0, 16) + "') ";
                        }
                    }

                    else
                        xpath = xpath + "@" + key + "='" + value + "' ";
                    count++;
                }
                xpath = xpath + "]";
            }
            return xpath;
        }

        public static string GetAttributeValue(this XElement e, string attrName)
        {
            if (e == null)
                return null;
            if (string.IsNullOrEmpty(attrName))
                return null;
            var xa = e.Attribute(attrName) ?? e.Attribute(attrName.ToLower());
            return xa == null ? null : xa.Value;
        }

        public static Dictionary<string, string> GetAttributeList(this XElement e)
        {
            Dictionary<string, string> attributeList = new Dictionary<string, string>();
            foreach (XAttribute attr in e.Attributes())
            {
                attributeList.Add(attr.Name.ToString(), attr.Value);
            }
            return attributeList;
        }

        public static string XElementToText(this XElement e)
        {
            if (e == null)
                return "<Result Result='Error' Reason='XElement is NULL' />";
            var retString = e.Name + "\n";
            if (!string.IsNullOrWhiteSpace(e.Value))
                retString += e.Value + "\n";
            return e.Attributes()
                    .Aggregate(retString, (current, a) => current + (" " + a.Name + " : " + a.Value + "\n"));
        }

        public static XElement GetRootElement(this XElement current)
        {
            var parent = current;
            while (true)
            {
                if (parent == null)
                    break;
                if (parent.Parent == null)
                    break;
                parent = parent.Parent;
            }
            return parent;
        }
    }
}