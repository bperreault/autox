package autox.actions;

import autox.config.Configuration;
import org.jdom.Attribute;
import org.jdom.Element;

import java.util.List;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 * Command string sample
 * <Command Action="autox.actions.SetEnv">
 * <Env host='sauce' />
 * <Env sauce.os='mac' />
 * ... ...
 * </Command>
 */
public class SetEnv extends Action {
    @Override
    public Object findTestObject() {
        return null;
    }

    @Override
    protected void handle(Object testObject) {
        for (Object element : getOriginal().getChildren("Env")) {
            List<Attribute> attributeList = ((Element) element).getAttributes();
            for (Attribute attribute : attributeList) {
                Configuration.getInstance().set(attribute.getName(), attribute.getValue());
            }
        }
    }
}
