﻿using System;
using System.Linq;
using System.Xml.Linq;
using AutoX.Basic;

namespace AutoX.DB
{
    public class DBUtils
    {
        public static string GetHtmlHeader()
        {
            return 
                "<html><head><title>Automation Results</title><meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\"><script language=\"javascript\">" +
                "function ShowOrUnShow(item){ if (!item.parentNode.getElementsByTagName(\"ul\")[0]) return; var x = item.parentNode.getElementsByTagName(\"ul\")[0];x.style.display = (x.style.display == \"\") ? 'none' : \"\"; }  "+
                "function ExpandAll(){var e = document.getElementsByTagName(\"ul\");for ( var i = 0, len = e.length; i < len; i++ ){e[i].style.display = \"\";}}"+
                "function CollapseAll(){var e = document.getElementsByTagName(\"ul\");for ( var i = 0, len = e.length; i < len; i++ ){e[i].style.display = \"none\";}}"+
                "var popUpWin = 0;"+
                "function popUpWindow(content){if(popUpWin){if(!popUpWin.closed) popUpWin.close();}popUpWin = window.open('','','width=800,height=600,resizeable,scrollbars');var data=\"<html><body><img src='data:image/jpg;base64,\"+content.getAttribute('snapshot')+\"' /></body></html>\";popUpWin.document.write(data);}"+
                "</script></head><a href=\"#\" onclick=\"ExpandAll()\" >Expand</a><span>    </span><a href=\"#\" onclick=\"CollapseAll()\">Collapse</a><div>";
        }

        public static string GetHtmlFooter()
        {
            return "</div></html>";
        }

        public static string ExportReulstToHtml(XElement xElement)
        {
            var toBeExportdId = xElement.GetAttributeValue(Constants._ID);
            var tag = xElement.Name.ToString();
            if (tag.Equals(Constants.RESULT) || tag.Contains("." + Constants.RESULT))
            {
                //this is a result node
                var name = xElement.GetAttributeValue(Constants.NAME);
                var origianl = xElement.GetAttributeValue("Original");
                var final = xElement.GetAttributeValue("Final");
                if (String.IsNullOrEmpty(final))
                    final = "&nbsp";
                var created = xElement.GetAttributeValue("Created");
                var updated = xElement.GetAttributeValue("Updated");
                var authors = xElement.GetAttributeValue("Authors");
                var description = xElement.GetAttributeValue("Description");
                var color = final.Equals("Success") ? "GREEN" : "RED";
                var ret =
                    "<li><span title=\"Start:" + created + "&#10End:" + updated + "\"   onclick=\"ShowOrUnShow(this)\"> " 
                    + name + " </span> <span title=\"Original:" + origianl + "&#10Final:" + final + "\" style=\"color:" + color + "\" > " + final
                    + " </span><ul  style=\"display:none\"><span>Authors: "+authors+" Description: "+description+"</span>";
                var children = DBFactory.GetData().GetChildren(toBeExportdId);
                var kidTag =children.Descendants().First().Name.ToString();
                if (kidTag.Equals(Constants.RESULT) || kidTag.Contains("." + Constants.RESULT))
                {
                    foreach (var kid in children.Descendants())
                    {
                        ret += ExportReulstToHtml(kid);
                    }
                }
                else
                {
                    //add table header
                    ret +=
                        "<table cellspacing=\"0\" cellpadding=\"1\" border=\"1\"  width=\"100%\"><thead><tr id=\"headingrow\">" +
                        "<th  nowrap=\"nowrap\" >Action </th>" +
                        "<th  nowrap=\"nowrap\" >UI Object </th>" +
                        "<th  nowrap=\"nowrap\" >Data </th>" +
                        "<th  nowrap=\"nowrap\" >Result </th>" +
                        "<th  nowrap=\"nowrap\" >Duration </th>" +
                        "<th  nowrap=\"nowrap\" >Reason </th>" +"</tr></thead><tbody>";
                    foreach (var kid in children.Descendants())
                    {
                        ret += "<tr>";
                        var action = kid.GetAttributeValue("Action");
                        var uiobject = kid.GetAttributeValue("UIObject");
                        var data = kid.GetAttributeValue("Data");
                        var result = kid.GetAttributeValue("Result");
                        var duration = kid.GetAttributeValue("Duration");
                        var reason = kid.GetAttributeValue("Reason");
                        if (String.IsNullOrEmpty(reason))
                            reason = "&nbsp&nbsp";
                        var snapshot = kid.GetAttributeValue("Link");
                        var starttime = kid.GetAttributeValue("StartTime");
                        var endtime = kid.GetAttributeValue("EndTime");
                        var id = kid.GetAttributeValue("_id");
                        var uiid = kid.GetAttributeValue("UIId");
                        var colour = result.Equals("Success") ? "GREEN" : "RED";
                        ret += "<td title=\""+id+"\">"+action+"</td>";
                        ret += "<td title=\"" + uiid + "\">" + uiobject + "</td>";
                        ret += "<td>"+data+"</td>";
                        ret += "<td align=\"center\" style=\"color:" + colour + "\">" + result.ToUpper() + "</td>";
                        ret += "<td align=\"right\" title=\"Start At:" + starttime+"&#10End At:"+endtime + "\">" + duration + "</td>";
                        if(snapshot.Length>100)
                            ret += "<td><a href=\"#\" onclick=\"javascript:popUpWindow(this)\" snapshot=\""+snapshot+"\">"+reason+"</a></td>";
                        else
                            ret += "<td>"+reason+"</td>";
                        ret += "</tr>";
                    }
                    //add table footer
                    ret += "</tbody></table>";
                }
                return ret + "</ul></li>";
            }
            return "";
        }
    }
}