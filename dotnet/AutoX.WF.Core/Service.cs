using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services;
using System.Xml.Linq;
using AutoX.Basic;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace AutoX.WF.Core
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet]
        string Hello(string s);

        [OperationContract]
        [WebGet]
        string Command(string s);
    }

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Service : WebService,IService
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
            Log.Debug(xmlFormatCommand);

            return ActionsFactory.Handle(xmlFormatCommand);
        }
    }
}
