#region

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

#endregion

namespace AutoX.Client.Core
{
    internal class SauceDriver : RemoteWebDriver
    {
        public SauceDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities, new TimeSpan(0, 10, 0))
        {
        }

        public string GetSessionId()
        {
            return SessionId.ToString();
        }
    }
}