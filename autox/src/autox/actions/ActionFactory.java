package autox.actions;

import autox.log.Log;
import org.jdom.Element;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.util.List;

/**
 * Created with AutoX
 * User: jien.huang
 * Date: 12/14/12
 */
public class ActionFactory {

    public static final String RESULT_TAG = "Result";
    public static final String ACTION_ATTRIBUTE_NAME = "Action";

    public static String handle(Element element){
        Element result = new Element(RESULT_TAG);
        List<Element> children = element.getChildren();
        for(Element step : children) {
            Action action = getAction(step);
            result.addContent(action.deal().toElement());
        }
        return result.toString();
    }

    private static Action getAction(Element element)  {
        String actionName = element.getAttributeValue(ACTION_ATTRIBUTE_NAME);

        Class<Action> c = null;
        Action action = null;
        try {
            c = (Class<Action>) Class.forName(actionName);
            Constructor<Action> constructor = c.getConstructor();
            action = constructor.newInstance();

        } catch (Exception e) {
            Log.error(e.getMessage(),e);
            action = new UnknownAction();
        }

        action.setOriginal(element);
        return action;
    }

}
