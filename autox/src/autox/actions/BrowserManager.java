package autox.actions;

import org.w3c.dom.Element;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
public class BrowserManager {
    private static BrowserManager ourInstance = new BrowserManager();

    public static BrowserManager getInstance() {
        return ourInstance;
    }

    private BrowserManager() {
    }

    public Browser getLatestBrowser(){
        return null;
    }

    public Browser getBrowser(String browserTitle){
        return  null;
    }

    public Browser getBrowser(Element element){
        return null;
    }
}
