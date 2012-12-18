package autox.actions;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/17/12
 *
 * <Step Action="autox.actions.Close" />
 */
public class Close extends Action {
    @Override
    public Object findTestObject() {
        return null;
    }

    @Override
    protected void handle(Object testObject) {
        BrowserManager.getInstance().close();
    }
}
