package autox.actions;

import org.apache.commons.lang.StringUtils;
import org.openqa.selenium.WebElement;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
public class Check extends Action {
    @Override
    public Object findTestObject() {
        return findUIObject();
    }

    @Override
    protected void handle(Object testObject) {
        if(testObject==null){
            getResult().Error("The target want to set text to not found!");
            return;
        }
        String data = getAttributeData();
        boolean toBe = false;
        if(StringUtils.isEmpty(data)||data.equalsIgnoreCase("true")||data.equalsIgnoreCase("yes")||data.equalsIgnoreCase("y")||data.equalsIgnoreCase("on"))
            toBe = true;

        if(((WebElement)testObject).isSelected()!=toBe)
            ((WebElement)testObject).click();
    }
}
