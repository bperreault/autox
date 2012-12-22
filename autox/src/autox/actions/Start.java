package autox.actions;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 *
 * <Step Action="autox.actions.Start" />
 */
public class Start extends Action {
    @Override
    public Object findTestObject() {
        return null;
    }

    @Override
    protected void handle(Object testObject) {

        BrowserManager.getInstance().start();

    }
}
