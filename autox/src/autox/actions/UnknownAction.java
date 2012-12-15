package autox.actions;

import org.jdom.Element;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/14/12
 */
public class UnknownAction extends Action {
    @Override
    public Object findTestObject() {
        return null;
    }

    @Override
    protected Result handle(Object testObject) {
        Result result = new Result(getOriginal());
        result.Error("We receive an unknown action.");
        return result;
    }
}
