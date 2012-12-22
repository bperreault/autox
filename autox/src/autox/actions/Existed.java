package autox.actions;

import org.apache.axis.utils.StringUtils;
import org.openqa.selenium.WebElement;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
public class Existed extends Action {
    @Override
    public Object findTestObject() {
        return findUIObject();
    }

    @Override
    protected void handle(Object testObject) {
        if(testObject==null){
            getResult().Error("The expected object is not existed!");
            return;
        }

    }
}
