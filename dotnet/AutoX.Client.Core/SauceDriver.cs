using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoX.Client.Core
{
    class SauceDriver : RemoteWebDriver
    {
        
        public SauceDriver(Uri remoteAddress, ICapabilities desiredCapabilities) : base(remoteAddress, desiredCapabilities)
        {
        }

        public string GetSessionId()
        {
            return base.SessionId.ToString();
        }
    }
}
