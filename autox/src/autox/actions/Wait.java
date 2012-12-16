package autox.actions;


import autox.log.Log;
import org.apache.commons.lang.StringUtils;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 * <p/>
 * Command string sample
 * <Command Action="autox.actions.Wait" Data="10" />
 */

public class Wait extends Action {

    private static final int INTERVAL = 1000;
    private static final String DATA_ATTRIBUTE_NAME = "Data";

    @Override
    public Object findTestObject() {
        return null;
    }

    @Override
    protected void handle(Object testObject) {
        String data = getOriginal().getAttributeValue(DATA_ATTRIBUTE_NAME);
        if (StringUtils.isEmpty(data)) {
            data = "17";
        }
        try {
            Thread.sleep(Long.parseLong(data) * INTERVAL);
        } catch (InterruptedException e) {
            Log.error(e.getMessage(), e);

            getResult().Error(e.getMessage());
            return;
        }
        getResult().Success();
    }
}
