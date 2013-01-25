using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;

namespace AutoX.Client.Core
{
    class SauceDriver : RemoteWebDriver
    {
        
        public SauceDriver(Uri remoteAddress, ICapabilities desiredCapabilities) : base(remoteAddress, desiredCapabilities)
        {
        }

        public string GetSessionId()
        {
            return SessionId.ToString();
        }
    }
}
