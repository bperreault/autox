package autox.actions;

import autox.config.Configuration;
import autox.log.Log;
import org.openqa.selenium.Platform;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;
import org.openqa.selenium.firefox.FirefoxDriver;
import org.openqa.selenium.ie.InternetExplorerDriver;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.openqa.selenium.remote.RemoteWebDriver;
import org.w3c.dom.Element;

import java.net.MalformedURLException;
import java.net.URL;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
public class BrowserManager {
    private static BrowserManager ourInstance = new BrowserManager();
    private WebDriver driver;

    private BrowserManager() {
    }

    public static BrowserManager getInstance() {
        return ourInstance;
    }

    public WebDriver getLatestBrowser() {
        //TODO temp solution
        return driver;
    }

    public WebDriver getBrowser(String browserTitle) {
        //TODO temp solution
        return driver;
    }

    public WebDriver getBrowser(Element element) {
        //TODO temp solution
        return driver;
    }

    public void start() {
        //read config.properties, start a browser
        if (Configuration.getInstance().get("browser.host", "local").equalsIgnoreCase("sauce")) {
            startSauceBrowser();
        } else {
            startLocalBrowser();
        }
    }

    private void startLocalBrowser() {

        String browserName = Configuration.getInstance().get("local.browser", "firefox");
        DesiredCapabilities capabilities = null;
        if (browserName.equalsIgnoreCase("chrome")) {
            System.setProperty("webdriver.chrome.driver", System.getProperty("user.dir") + "/autox/chromedriver");
            DesiredCapabilities chromeCapabilities = DesiredCapabilities.chrome();
            ChromeOptions options = new ChromeOptions();
            chromeCapabilities.setCapability(ChromeOptions.CAPABILITY, options);
            capabilities = chromeCapabilities;
            driver = new ChromeDriver(options);

        }

        if (browserName.equalsIgnoreCase("firefox")) {
            capabilities = DesiredCapabilities.firefox();
            driver = new FirefoxDriver(capabilities);
        }

        if (browserName.equalsIgnoreCase("iexplorer")) {
            capabilities = DesiredCapabilities.internetExplorer();
            capabilities.setCapability(InternetExplorerDriver.INTRODUCE_FLAKINESS_BY_IGNORING_SECURITY_DOMAINS, true);
            driver = new InternetExplorerDriver(capabilities);
        }
        if (capabilities == null)
            throw new RuntimeException(String.format("Do not support this local browser: %s", browserName));
        String url = Configuration.getInstance().get("test.url", "about:blank");
        driver.get(url);
    }

    private void startSauceBrowser() {
        DesiredCapabilities capabilities = new DesiredCapabilities();
        capabilities.setBrowserName(Configuration.getInstance().get("sauce.browser", "firefox"));
        capabilities.setCapability("version", Configuration.getInstance().get("sauce.browser.version", "17"));

        capabilities.setCapability("platform", Platform.valueOf(Configuration.getInstance().get("sauce.os", "WINDOWS")));
        capabilities.setCapability("name", "autox");
        try {
            driver = new RemoteWebDriver(
                    new URL("http://" + Configuration.getInstance().get("sauce.user", "No User")
                            + ":" + Configuration.getInstance().get("sauce.key", "No key")
                            + "@ondemand.saucelabs.com:80/wd/hub"),
                    capabilities);
        } catch (MalformedURLException e) {
            Log.fatal(e.getMessage(), e);
        }
        String url = Configuration.getInstance().get("test.url", "about:blank");

        driver.get(url);
    }

    public void close() {

        driver.quit();
    }
}
