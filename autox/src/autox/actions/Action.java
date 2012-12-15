package autox.actions;

import org.jdom.Element;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/14/12
 */
public abstract class Action {
    public Element getOriginal() {
        return original;
    }

    public void setOriginal(Element original) {
        this.original = original;
    }

    private Element original;
    public abstract Object findTestObject();
    public Result deal(){
        Object testObject = findTestObject();
        Result result = handle(testObject);
        return result;
    }

    protected abstract Result handle(Object testObject);


}
