package autox.actions;

import autox.log.Log;
import org.jdom.Element;
import org.openqa.selenium.WebElement;


/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
public class Click extends Action {
    @Override
    public Object findTestObject() {
        return findUIObject();
    }

    @Override
    protected void handle(Object testObject) {
        if(testObject==null){
            getResult().Error("The target want to click not found!");
        }

        ((WebElement)testObject).click();
    }
}
