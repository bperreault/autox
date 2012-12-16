package autox.actions;

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
    protected void handle(Object testObject) {

        result.Error("We receive an unknown action.");

    }
}
