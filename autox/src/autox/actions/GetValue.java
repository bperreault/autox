package autox.actions;

import org.apache.commons.lang.StringUtils;
import org.openqa.selenium.WebElement;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
public class GetValue extends Action {
    String value = null;
    String propertyName = "value";
    String prefix="",postFix="";
    int occur = 0;
    @Override
    public Object findTestObject() {
        if(!StringUtils.isEmpty(getOriginal().getAttributeValue("Property")))
            return findUIObject();
        return null;
    }

    @Override
    protected void handle(Object testObject) {

        if(testObject==null){
            //Simulate performance tools to find a string


        }   else{
            ((WebElement)testObject).getAttribute(propertyName);
        }

    }
}
