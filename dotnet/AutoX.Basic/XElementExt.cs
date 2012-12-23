// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Linq;
using System.Xml.Linq;

#endregion

namespace AutoX.Basic
{
    public static class XElementExt
    {
        public static void DealExtra(this XElement rElement)
        {
            XAttribute xAttribute = rElement.Attribute("EXTRA");
            if (xAttribute == null) return;
            string eData = xAttribute.Value;
            if (!string.IsNullOrEmpty(eData))
            {
                XElement xExtra = XElement.Parse(eData);
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
            string original = xUi.GetAttributeValue("XPath");
            if (!string.IsNullOrEmpty(original))
                return original;
            string tag = "//*";
            XAttribute xTag = xUi.Attribute("tag");
            if (xTag != null)
            {
                tag = "//" + xTag.Value;
                xTag.Remove();
            }
            string xpath = tag;
            if (xUi.Attributes().Any())
            {
                xpath = xpath + "[";
                int count = 0;
                foreach (XAttribute xa in xUi.Attributes())
                {
                    string key = xa.Name.ToString();
                    string value = xa.Value;
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
            XAttribute xa = e.Attribute(attrName) ?? e.Attribute(attrName.ToLower());
            return xa == null ? null : xa.Value;
        }

        public static string XElementToText(this XElement e)
        {
            if (e == null)
                return "<Result Result='Error' Reason='XElement is NULL' />";
            string retString = e.Name + "\n";
            if (!string.IsNullOrWhiteSpace(e.Value))
                retString += e.Value + "\n";
            return e.Attributes().Aggregate(retString, (current, a) => current + (" " + a.Name + " : " + a.Value + "\n"));
        }

        public static XElement GetRootElement(this XElement current)
        {
            XElement parent = current;
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