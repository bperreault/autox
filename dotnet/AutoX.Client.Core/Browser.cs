// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using AutoX.Basic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    public sealed class Browser : IDisposable
    {
        //CSS types that we care
        private readonly string _cssxPath = Configuration.Settings("CSSType",
                                                                   "//*[@class='ROW1' or @class='ROW2' or @class='EDIT' or @class='VIEWBOXCAPTION' or @class='TABON' or @class='TABOFF' or @class='ButtonItem' or @class='TOPBC' or @type='checkbox' or @type='text' or @class='Logonbutton' or @type='password']");

        private readonly Hashtable _pool = new Hashtable();

        private IWebDriver _browser;
        private readonly Config _config;

        public Browser(Config config)
        {
            _config = config;
        }

        #region IDisposable Members

        public void Dispose()
        {
            CloseBrowser();
        }

        #endregion

        public IWebDriver GetCurrentBrowser()
        {
            if (_browser == null)
            {
                CloseBrowser();
                StartBrowser();
            }
//            DismissUnexpectedAlert();
            if (_browser != null)
            {
                var bs = _browser.WindowHandles;
                _browser.SwitchTo().Window(bs.Count == 1 ? bs[0] : bs[bs.Count - 1]);
            }

            return _browser;
        }

        public string GetAllValuableObjects()
        {
            var xBrowser = new XElement(Constants.UI_OBJECT);
            xBrowser.SetAttributeValue(Constants._TYPE, Constants.UI_OBJECT);
            xBrowser.SetAttributeValue("type", "Browser");
            xBrowser.SetAttributeValue(Constants.NAME, "Browser");
            var valueObjects = GetCurrentBrowser().FindElements(By.XPath(_cssxPath));

            foreach (IWebElement webElement in valueObjects)
            {
                xBrowser.Add(GetXFromElement(webElement));
            }
            var frames = GetCurrentBrowser().FindElements(By.XPath("//frame"));
            if (frames.Count > 0)
            {
                var framesNames = new string[frames.Count];
                var indexer = 0;
                foreach (IWebElement webElement in frames)
                {
                    var frameName = webElement.GetAttribute(Constants._NAME);
                    framesNames[indexer] = frameName;
                    indexer++;
                }
                foreach (string name in framesNames)
                {
                    var frameX = new XElement("frame");
                    frameX.SetAttributeValue(Constants._NAME, name);
                    GetAllValueableObjectsOfFrame(frameX);
                    _browser.SwitchTo().DefaultContent();
                    _browser.SwitchTo().Frame(name);
                    xBrowser.Add(frameX);
                }
            }
            return xBrowser.ToString(SaveOptions.None);
        }

        public void GetAllValueableObjectsOfFrame(XElement parent)
        {
            //XElement xParent = GetXFromElement(parent);

            var valueObjects = _browser.FindElements(By.XPath(_cssxPath));
            foreach (IWebElement webElement in valueObjects)
            {
                parent.Add(GetXFromElement(webElement));
            }
        }

        private static XElement GetXFromElement(IWebElement webElement)
        {
            if (webElement == null)
                return null;

            var xe = new XElement(Constants.UI_OBJECT);
            xe.SetAttributeValue(Constants._TYPE, Constants.UI_OBJECT);
            var eTag = webElement.TagName;
            xe.SetAttributeValue("type", !string.IsNullOrEmpty(eTag) ? eTag : "*");
            var eText = webElement.Text;
            if (!string.IsNullOrEmpty(eText))
            {
                xe.SetAttributeValue("text", eText);
            }

            WebAttrToAttr(webElement, xe, "id");
            WebAttrToAttr(webElement, xe, Constants._NAME);
            WebAttrToAttr(webElement, xe, Constants._NAME);
            WebAttrToAttr(webElement, xe, "type");
            WebAttrToAttr(webElement, xe, "value");
            WebAttrToAttr(webElement, xe, "class");

            var eSrc = webElement.GetAttribute("src");
            if ((eTag.Equals("img") || eTag.Equals("img")) && !string.IsNullOrEmpty(eSrc))
                xe.SetAttributeValue("src", eSrc);
            var eHref = webElement.GetAttribute("href");
            if (eTag.Equals("a") && !string.IsNullOrEmpty(eHref))
                xe.SetAttributeValue("href", eHref);
            var xpath = xe.GenerateXPathFromXElement();
            xe.SetAttributeValue("XPath", xpath);
            var nAttribute = xe.Attribute(Constants._NAME);
            if (nAttribute != null)
            {
                var value = nAttribute.Value;
                nAttribute.Remove();
                xe.SetAttributeValue(Constants.NAME, value);
            }
            else
            {
                var id = xe.GetAttributeValue("id");
                var value = xe.GetAttributeValue("value");
                var name = xe.GetAttributeValue("name");
                if (!string.IsNullOrEmpty(id))
                {
                    xe.SetAttributeValue(Constants.NAME, "Id_" + id);
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    xe.SetAttributeValue(Constants.NAME, name);
                }
                else if (!string.IsNullOrEmpty(value))
                {
                    xe.SetAttributeValue(Constants.NAME, value);
                }
                else if (!string.IsNullOrEmpty(eText))
                {
                    xe.SetAttributeValue(Constants.NAME, eText);
                }
                else
                {
                    xe.SetAttributeValue(Constants.NAME, "EmptyName");
                }
            }
            return xe;
        }

        private static void WebAttrToAttr(IWebElement webElement, XElement xe, string attrName)
        {
            var eId = webElement.GetAttribute(attrName);
            if (!string.IsNullOrEmpty(eId))
                xe.SetAttributeValue(attrName, eId);
        }

        public IWebDriver SwitchToAnotherBrowser()
        {
            if (_browser == null)
            {
                CloseBrowser();
                StartBrowser();
            }
            DismissUnexpectedAlert();
            if (_browser != null)
            {
                var bs = _browser.WindowHandles;
                if (bs.Count == 1)
                {
                    _browser.SwitchTo().Window(bs[0]);
                    return _browser;
                }
                var currentHandle = _browser.CurrentWindowHandle;
                foreach (string handle in bs)
                {
                    if (!currentHandle.Equals(handle))
                    {
                        _browser.SwitchTo().Window(handle);
                        break;
                    }
                }
            }

            return _browser;
        }

        //CSStype subpool<fingerprint string, webelement>

        public void Prepare()
        {
        }

        public void Clear()
        {
            foreach (object item in _pool.Values)
            {
                if (item.GetType().Name.Contains("Hashtable"))
                {
                    ((Hashtable)item).Clear();
                }
            }
            _pool.Clear();
        }

        public ReadOnlyCollection<IWebElement> GetWebElement(XElement xPage)
        {
            var xParent = xPage.Name.ToString();
            XElement xUI;
            if (xParent.Equals("Browser") || xParent.Equals("frame"))
            {
                xUI = xPage.Elements().First();
                GetCurrentBrowser().SwitchTo().DefaultContent();
                if (xParent.Equals("frame"))
                {
                    var frame = xPage.Attribute(Constants._NAME);
                    if (frame != null)
                    {
                        var frameName = frame.Value;
                        GetCurrentBrowser().SwitchTo().Frame(frameName);
                    }
                }
            }
            else
                xUI = xPage;

            var xpath = xUI.GenerateXPathFromXElement();
            return GetCurrentBrowser().FindElements(By.XPath(xpath));
        }

        public string Snapshot()
        {
            return ((ITakesScreenshot)GetCurrentBrowser()).GetScreenshot().AsBase64EncodedString;

            //IJavaScriptExecutor js = GetCurrentBrowser() as IJavaScriptExecutor;
            //Response screenshotResponse = js.ExecuteScript(DriverCommand.Screenshot, null);
            //return screenshotResponse.Value.ToString();
        }

        public void DismissUnexpectedAlert()
        {
            var alert = GetAlert();
            if (alert != null)
            {
                alert.Dismiss();
                Log.Warn("Close an unexpected dialog, please check.");
            }
        }

        private IAlert GetAlert()
        {
            IAlert alert = null;
            try
            {
                alert = _browser.SwitchTo().Alert();
            }
            catch (Exception)
            {
                Log.Debug("Suppress get alert");
            }
            return alert;
        }

        public void StartBrowser()
        {
            var clientType = _config.Get("HostType", "Sauce");
            if (String.Compare(clientType, "Sauce", StringComparison.OrdinalIgnoreCase) == 0)
                StartSauceBrowser();
            else
                StartLocalBrowser();
        }

        private void StartSauceBrowser()
        {
            DesiredCapabilities capabillities = ConfigSauceCapabilities();
            _browser = new SauceDriver(
                      new Uri("http://ondemand.saucelabs.com:80/wd/hub"), capabillities);

            _browser.Navigate().GoToUrl(_config.Get("DefaultURL", "about:blank"));
            MaximiseBrowser();
            _sId = ((SauceDriver)_browser).GetSessionId();
        }

        private DesiredCapabilities ConfigSauceCapabilities()
        {
            DesiredCapabilities capabillities;
            var browserType = _config.Get("BrowserType", "Firefox");
            if (browserType.Equals("IE"))
                capabillities = DesiredCapabilities.InternetExplorer();
            else if (browserType.Equals("Chrome"))
                capabillities = DesiredCapabilities.Chrome();
            else if (browserType.Equals("Android"))
                capabillities = DesiredCapabilities.Android();
            else if (browserType.Equals("Ipad"))
                capabillities = DesiredCapabilities.IPad();
            else if (browserType.Equals("Iphone"))
                capabillities = DesiredCapabilities.IPhone();
            else if (browserType.Equals("Opera"))
                capabillities = DesiredCapabilities.Opera();
            else if (browserType.Equals("Safari"))
                capabillities = DesiredCapabilities.Safari();
            else if (browserType.Equals("InternetExplorer"))
                capabillities = DesiredCapabilities.InternetExplorer();
            else
                capabillities = DesiredCapabilities.Firefox();
                var _browserVersion =  _config.Get("BrowserVersion");
                if(!string.IsNullOrEmpty(_browserVersion))
                    capabillities.SetCapability(CapabilityType.Version, _browserVersion);
            capabillities.SetCapability(CapabilityType.Platform, _config.Get("BrowserPlatform", "Windows 2008"));
            var versionName = _config.Get("AUTVersion") ?? "Test.Version";

            var buildName = _config.Get("AUTBuild") ?? "Test.Build";
            capabillities.SetCapability(Constants._NAME, _config.Get("SauceName", versionName+"/"+buildName));
            capabillities.SetCapability("username", _config.Get("SauceUserName", "autox"));
            capabillities.SetCapability("accessKey", _config.Get("SauceAccessKey", "b3842073-5a7a-4782-abbc-e7234e09f8ac"));
            capabillities.SetCapability("idle-timeout", 300);
            capabillities.SetCapability("max-duration", 3600);
            capabillities.SetCapability("command-timeout", 300);
            return capabillities;
        }

        private string _sId;

        public string GetResultLink()
        {
            var clientType = _config.Get("HostType", "Sauce");
            if (String.Compare(clientType, "Sauce", StringComparison.OrdinalIgnoreCase) == 0){
                if (string.IsNullOrEmpty(_sId))
                return null;
            if (_config.Get("SauceFree", "true").ToLower().Equals("true"))
            {
                return "https://saucelabs.com/tests/" + _sId;
            }
            var key = _config.Get("SauceUser", "autox") + ":" +
                      _config.Get("SauceKey", "b3842073-5a7a-4782-abbc-e7234e09f8ac");
            var jobId = AsymmetricEncryption.Hmacmd5(key, _sId);
            return "https://saucelabs.com/jobs/" + _sId + "?auth=" + jobId;
            }else{
                return Snapshot();
            }
            
        }

        private void StartLocalBrowser()
        {
            var browserType = _config.Get("BrowserType", "InternetExplorer");
            if (browserType.Equals("IE") || browserType.Equals("InternetExplorer"))
            {
                var processor = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                if (processor != null && !processor.Contains("x86"))
                {
                    Environment.SetEnvironmentVariable("webdriver.ie.driver",
                                                       Directory.GetCurrentDirectory() + "\\IEDriverServer64.exe");
                    File.Copy(Directory.GetCurrentDirectory() + "\\IEDriverServer64.exe",
                              Directory.GetCurrentDirectory() + "\\IEDriverServer.exe");
                }

                else
                {
                    Environment.SetEnvironmentVariable("webdriver.ie.driver",
                                                       Directory.GetCurrentDirectory() + "\\IEDriverServer32.exe");
                    File.Copy(Directory.GetCurrentDirectory() + "\\IEDriverServer32.exe",
                              Directory.GetCurrentDirectory() + "\\IEDriverServer.exe");
                }
                var capabilities = DesiredCapabilities.InternetExplorer();

                _browser = new InternetExplorerDriver();
            }

            if (browserType.Equals("Firefox"))
                _browser = new FirefoxDriver();
            if (browserType.Equals("Chrome"))
            {
                Environment.SetEnvironmentVariable("webdriver.ie.driver",
                                                   Directory.GetCurrentDirectory() + "\\chromedriver.exe");
                _browser = new ChromeDriver();
            }

            _browser.Navigate().GoToUrl(_config.Get("DefaultURL", "about:blank"));
            MaximiseBrowser();
        }

        private void MaximiseBrowser()
        {
            if (_config.Get("MaximScreen", "True").Equals("True"))
                _browser.Manage().Window.Maximize();
        }

        public void CloseBrowser()
        {
            if (_browser != null)
            {
                _browser.Quit();
            }

            //browser = null;
            var clientType = _config.Get("HostType", "Sauce");
            if (String.Compare(clientType, "Sauce", System.StringComparison.OrdinalIgnoreCase) != 0)
            {
                if (_browser != null) _browser.Dispose();
                CloseLocalBrowser();
            }
            _browser = null;
        }

        private void CloseLocalBrowser()
        {
            var browserType = _config.Get("Browser.Type", "IE");
            if (browserType.Equals("IE"))

                Command.DosCommand(Environment.SystemDirectory + "\\taskkill.exe", " /IM iexplore.exe");
            if (browserType.Equals("Firefox"))
                Command.DosCommand(Environment.SystemDirectory + "\\taskkill.exe", " /IM firefox.exe");
            if (browserType.Equals("Chrome"))
                Command.DosCommand(Environment.SystemDirectory + "\\taskkill.exe", " /IM chrome.exe");
        }
    }
}
