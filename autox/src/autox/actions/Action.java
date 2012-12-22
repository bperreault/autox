package autox.actions;

import autox.config.Configuration;
import autox.log.Log;
import org.jdom.Element;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/14/12
 */
public abstract class Action {

    public static final String DATA_ATTRIBUTE = "Data";
    public static final String UI_OBJECT_TAG = "UIObject";

    public Element getOriginal() {
        return original;
    }

    public void setOriginal(Element original) {
        this.original = original;
        result = new Result(original);
    }

    private Element original;

    public Result getResult() {
        return result;
    }

    protected Result result;

    public abstract Object findTestObject();

    public void deal() {

        Object testObject = findTestObject();
        handle(testObject);
    }

    protected abstract void handle(Object testObject);

    public Object NotExpectedFindUIObject(){
        long timeOut = Long.parseLong(Configuration.getInstance().get("not.expected.timeout","3"));
        return findGuiTestObject(timeOut);
    }
    public Object findUIObject() {
        long timeOut = Long.parseLong(Configuration.getInstance().get("implicitly.timeout","30"));
        return findGuiTestObject(timeOut);
    }

    private Object findGuiTestObject(long timeOut) {
        Element uiObject = getOriginal().getChild(UI_OBJECT_TAG);
        if(uiObject==null){
            Log.warn("No UIObject element in the command, don't know how to find the target!");
            return null;
        }
        return Browser.findTestObject(uiObject,timeOut);
    }

    public String getAttributeData() {
        String data =  getOriginal().getAttributeValue(DATA_ATTRIBUTE);
        if(data == null)
            return "";
        return data;
    }
}
