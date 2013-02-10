// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using AutoX.Basic;
using AutoX.WF.Core;
using System.ComponentModel;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml.Linq;

#endregion

namespace AutoX.Web
{
    /// <summary>
    ///   Summary description for Service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]

    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    [ScriptService]
    public class Service : WebService
    {
        [WebMethod]
        public string Hello(string input)
        {
            return "You sent us :" + input;
        }

        /// <summary>
        ///   It only accept the XML format command
        /// </summary>
        /// <param name="xmlFormatCommand"> </param>
        /// <returns> </returns>
        [WebMethod]
        public string Command(string xmlFormatCommand)
        {
            return ActionsFactory.Handle(xmlFormatCommand).ToString(SaveOptions.None);
        }
    }
}