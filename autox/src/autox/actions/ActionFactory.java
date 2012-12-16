package autox.actions;

import autox.log.Log;
import autox.utils.XML;
import org.jdom.Element;

import java.lang.reflect.Constructor;
import java.util.List;

/**
 * Created with AutoX
 * User: jien.huang
 * Date: 12/14/12
 */
public class ActionFactory {

    public static final String RESULT_TAG = "Result";
    public static final String ACTION_ATTRIBUTE_NAME = "Action";

    public static String handle(String commandInfo) {
        XML xml = null;
        try {
            xml = new XML(commandInfo);
        } catch (Exception e) {
            Log.fatal(e.getMessage(), e);
            Result result = new Result(null);
            result.Error("We receive the command, but it is not in XML format:\n" + commandInfo);
            return result.toString();
        }
        Element element = xml.getRoot();
        return handle(element);
    }

    public static String handle(Element element) {
        Element result = new Element(RESULT_TAG);
        List<Element> children = element.getChildren();
        for (Element step : children) {
            Action action = getAction(step);
            action.deal();
            result.addContent(action.getResult().toElement());
        }
        return result.toString();
    }

    private static Action getAction(Element element) {
        String actionName = element.getAttributeValue(ACTION_ATTRIBUTE_NAME);

        Class<Action> c;
        Action action;
        try {
            c = (Class<Action>) Class.forName(actionName);
            Constructor<Action> constructor = c.getConstructor();
            action = constructor.newInstance();

        } catch (Exception e) {
            Log.error(e.getMessage(), e);
            action = new UnknownAction();
        }

        action.setOriginal(element);
        return action;
    }

}
