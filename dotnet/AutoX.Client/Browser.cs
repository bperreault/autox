// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AutoX.Basic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

#endregion

namespace AutoX.Client
{
    public sealed class Browser : IDisposable
    {
        private static Browser _instance;
        //CSS types that we care
        private readonly string _cssxPath = Configuration.Settings("CSSType",
                                                                   "//*[@class='ROW1' or @class='ROW2' or @class='EDIT' or @class='VIEWBOXCAPTION' or @class='TABON' or @class='TABOFF' or @class='ButtonItem' or @class='TOPBC' or @type='checkbox' or @type='text' or @class='Logonbutton' or @type='password']");

        private readonly Hashtable _pool = new Hashtable();

        private IWebDriver _browser;

        private Browser()
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            CloseBrowser();
        }

        #endregion

        public static Browser GetInstance()
        {
            return _instance ?? (_instance = new Browser());
        }

        public IWebDriver GetCurrentBrowser()
        {
            if (_browser == null)
            {
                CloseBrowser();
                StartBrowser();
            }
            DismissUnexpectedAlert();
            if (_browser != null)
            {
                ReadOnlyCollection<string> bs = _browser.WindowHandles;
                _browser.SwitchTo().Window(bs.Count == 1 ? bs[0] : bs[bs.Count - 1]);
            }

            return _browser;
        }

        public string GetAllValuableObjects()
        {
            var xBrowser = new XElement("UIObject");
            xBrowser.SetAttributeValue("Type", "Browser");
            xBrowser.SetAttributeValue("Name", "Browser");
            ReadOnlyCollection<IWebElement> valueObjects = GetCurrentBrowser().FindElements(By.XPath(_cssxPath));

            foreach (IWebElement webElement in valueObjects)
            {
                xBrowser.Add(GetXFromElement(webElement));
            }
            ReadOnlyCollection<IWebElement> frames = GetCurrentBrowser().FindElements(By.XPath("//frame"));
            if (frames.Count > 0)
            {
                var framesNames = new string[frames.Count];
                int indexer = 0;
                foreach (IWebElement webElement in frames)
                {
                    string frameName = webElement.GetAttribute("name");
                    framesNames[indexer] = frameName;
                    indexer++;
                }
                foreach (string name in framesNames)
                {
                    var frameX = new XElement("frame");
                    frameX.SetAttributeValue("name", name);
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

            ReadOnlyCollection<IWebElement> valueObjects = _browser.FindElements(By.XPath(_cssxPath));
            foreach (IWebElement webElement in valueObjects)
            {
                parent.Add(GetXFromElement(webElement));
            }
        }


        private static XElement GetXFromElement(IWebElement webElement)
        {
            if (webElement == null)
                return null;

            var xe = new XElement("UIObject");
            string eTag = webElement.TagName;
            xe.SetAttributeValue("Type", !string.IsNullOrEmpty(eTag) ? eTag : "*");
            string eText = webElement.Text;
            if (!string.IsNullOrEmpty(eText))
            {
                xe.SetAttributeValue("text", eText);
            }


            WebAttrToAttr(webElement, xe, "id");
            WebAttrToAttr(webElement, xe, "name");
            WebAttrToAttr(webElement, xe, "name");
            WebAttrToAttr(webElement, xe, "type");
            WebAttrToAttr(webElement, xe, "value");
            WebAttrToAttr(webElement, xe, "class");

            string eSrc = webElement.GetAttribute("src");
            if ((eTag.Equals("img") || eTag.Equals("img")) && !string.IsNullOrEmpty(eSrc))
                xe.SetAttributeValue("src", eSrc);
            string eHref = webElement.GetAttribute("href");
            if (eTag.Equals("a") && !string.IsNullOrEmpty(eHref))
                xe.SetAttributeValue("href", eHref);
            string xpath = xe.GenerateXPathFromXElement();
            xe.SetAttributeValue("XPath", xpath);
            XAttribute nAttribute = xe.Attribute("name");
            if (nAttribute != null)
            {
                string value = nAttribute.Value;
                nAttribute.Remove();
                xe.SetAttributeValue("Name", value);
            }
            else
            {
                string id = xe.GetAttributeValue("id");
                if (string.IsNullOrEmpty(id))
                {
                    xe.SetAttributeValue("Name", "EmptyName");
                }
                else
                {
                    xe.SetAttributeValue("Name", "Id_" + id);
                }
            }
            return xe;
        }

        private static void WebAttrToAttr(IWebElement webElement, XElement xe, string attrName)
        {
            string eId = webElement.GetAttribute(attrName);
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
                ReadOnlyCollection<string> bs = _browser.WindowHandles;
                if (bs.Count == 1)
                {
                    _browser.SwitchTo().Window(bs[0]);
                    return _browser;
                }
                string currentHandle = _browser.CurrentWindowHandle;
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
                    ((Hashtable) item).Clear();
                }
            }
            _pool.Clear();
        }

        public ReadOnlyCollection<IWebElement> GetWebElement(XElement xPage)
        {
            string xParent = xPage.Name.ToString();
            XElement xUI;
            if (xParent.Equals("Browser") || xParent.Equals("frame"))
            {
                xUI = xPage.Elements().First();
                GetInstance().GetCurrentBrowser().SwitchTo().DefaultContent();
                if (xParent.Equals("frame"))
                {
                    XAttribute frame = xPage.Attribute("name");
                    if (frame != null)
                    {
                        string frameName = frame.Value;
                        GetInstance().GetCurrentBrowser().SwitchTo().Frame(frameName);
                    }
                }
            }
            else
                xUI = xPage;

            string xpath = xUI.GenerateXPathFromXElement();


            return GetCurrentBrowser().FindElements(By.XPath(xpath));
        }

        public string Snapshot()
        {
            return ((ITakesScreenshot) GetCurrentBrowser()).GetScreenshot().AsBase64EncodedString;
            //IJavaScriptExecutor js = GetCurrentBrowser() as IJavaScriptExecutor;
            //Response screenshotResponse = js.ExecuteScript(DriverCommand.Screenshot, null);
            //return screenshotResponse.Value.ToString();
        }

        private void DismissUnexpectedAlert()
        {
            IAlert alert = GetAlert();
            if (alert != null)
            {
                alert.Dismiss();
                Logger.GetInstance().Log().Warn("Close an unexpected dialog, please check.");
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
                Logger.GetInstance().Log().Debug("Suppress get alert");
            }
            return alert;
        }

        private void StartBrowser()
        {
            string browserType = Configuration.Settings("BrowserType", "IE");
            if (browserType.Equals("IE"))
            {
                string processor = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
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
                DesiredCapabilities capabilities = DesiredCapabilities.InternetExplorer();

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

            _browser.Navigate().GoToUrl(Configuration.Settings("DefaultURL", "about:blank"));
            MaximiseBrowser();
        }

        private void MaximiseBrowser()
        {
            if (Configuration.Settings("MaximScreen", "True").Equals("True"))
                _browser.Manage().Window.Maximize();
        }

        public void CloseBrowser()
        {
            if (_browser != null)
            {
                _browser.Quit();
                _browser.Dispose();
            }

            //browser = null;

            string browserType = Configuration.Settings("BrowserType", "IE");
            if (browserType.Equals("IE"))

                DosCommand(Environment.SystemDirectory + "\\taskkill.exe", " /IM iexplore.exe");
            if (browserType.Equals("Firefox"))
                DosCommand(Environment.SystemDirectory + "\\taskkill.exe", " /IM firefox.exe");
            if (browserType.Equals("Chrome"))
                DosCommand(Environment.SystemDirectory + "\\taskkill.exe", " /IM chrome.exe");
        }

        private static void DosCommand(string cmd, string param)
        {
            var proc = new Process
                           {
                               StartInfo =
                                   {
                                       UseShellExecute = false,
                                       CreateNoWindow = true,
                                       WindowStyle = ProcessWindowStyle.Hidden,
                                       FileName = cmd,
                                       Arguments = param,
                                       RedirectStandardError = false,
                                       RedirectStandardOutput = false
                                   }
                           };
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();
        }

        public static void DosCommand(string param)
        {
            DosCommand("cmd", " /c " + param);
        }
    }
}