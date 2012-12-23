// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Net;
using System.Xml.Linq;

#endregion

namespace AutoX.Basic.Model
{
    public class Computer
    {
        public string ComputerName { get; set; }
        public string IPAddress { get; set; }
        public string Version { get; set; }

        public static Computer GetLocalHost()
        {
            var computer = new Computer
                               {
                                   ComputerName = Dns.GetHostName(),
                                   Version = Environment.OSVersion.VersionString,
                                   IPAddress = LocalIPAddress()
                               };
            return computer;
        }

        private static string LocalIPAddress()
        {
            string localIP = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        public XElement ToXElement()
        {
            //return this.GetXElementFromObject();
            var ret = new XElement("Computer");
            if (!string.IsNullOrEmpty(ComputerName))
            {
                ret.SetAttributeValue("ComputerName", ComputerName);
            }
            if (!string.IsNullOrEmpty(IPAddress))
            {
                ret.SetAttributeValue("IPAddress", IPAddress);
            }
            if (!string.IsNullOrEmpty(Version))
            {
                ret.SetAttributeValue("Version", Version);
            }
            return ret;
        }

        public static Computer FromXElement(XElement xComputer)
        {
            //return xComputer.GetObjectFromXElement() as Computer;
            var computer = new Computer
                               {
                                   ComputerName = xComputer.GetAttributeValue("ComputerName"),
                                   IPAddress = xComputer.GetAttributeValue("IPAddress"),
                                   Version = xComputer.GetAttributeValue("Version")
                               };

            return computer;
        }
    }
}