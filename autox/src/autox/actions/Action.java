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


}
