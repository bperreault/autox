package autox.actions;

import org.openqa.selenium.WebElement;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
public class Enter extends Action {
    @Override
    public Object findTestObject() {
        return findUIObject();
    }

    @Override
    protected void handle(Object testObject) {
        if(testObject==null){
            getResult().Error("The target want to set text to not found!");
        }
        String data = getAttributeData();
        //TODO need to handle file upload field, it cannot be cleared.
        ((WebElement)testObject).clear();
        ((WebElement)testObject).sendKeys(data);
    }

}
