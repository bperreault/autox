package autox.actions;

import org.apache.commons.collections.CollectionUtils;
import org.jdom.Attribute;
import org.jdom.Element;
import org.openqa.selenium.By;
import org.openqa.selenium.SearchContext;
import org.openqa.selenium.WebElement;

import java.util.List;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
public class Browser {
    /**
     * Find the 1st one matched
     * @param element Command XML format element
     * @return WebElement test object, null for not found
     */
    public static WebElement findTestObject(Element element){
        return findTestObject(element,0);
    }

    public static WebElement findTestObject(Element element, int nth){
        List<WebElement> found = find(BrowserManager.getInstance().getLatestBrowser(),element);
        if(found ==null)
            return null;
        if(found.size()==0)
            return null;
        if(found.size()<=nth)
            return null;
        return found.get(nth);
    }

    private static List<WebElement> find(SearchContext searchContext,Element element){
        List<WebElement> found = null ;
        for (Object attribute : element.getAttributes()) {
            By way = findWay((Attribute) attribute);
            if (way == null)
                continue;
            List<WebElement> findings =searchContext.findElements(way);
            if(findings.size()==0)
                return null;
            if(found==null){
                found = findings;
                continue;
            }
            found = (List<WebElement>) CollectionUtils.intersection(found, findings);
            if(found.size()==0)
                return null;

        }

        return found;
    }
    private static By findWay(Attribute attribute) {
        String attributeName = attribute.getName();
        //that means reserved attribute
        if (attributeName.startsWith("r-"))
            return null;
        String attributeValue = attribute.getValue();
        if (attributeValue.isEmpty())
            throw new IllegalArgumentException("Cannot find elements when " + attributeName + " is null.");
//        if (attributeValue.toLowerCase().contains("regularexpression("))
//            return ObjectFinder.Regex(attributeName, attributeValue);
//        if (attributeName.equalsIgnoreCase("TextNode"))
//            return ObjectFinder.TextNode(attributeValue);
        if (attributeName.equalsIgnoreCase("id"))
            return By.id(attributeValue);
        if (attributeName.equalsIgnoreCase("tagname"))
            return By.tagName(attributeValue);
        if (attributeName.equalsIgnoreCase("xpath"))
            return By.xpath(attributeValue);
        if (attributeName.equalsIgnoreCase("classname"))
            return By.className(attributeValue);
        if (attributeName.equalsIgnoreCase("cssselector"))
            return By.cssSelector(attributeValue);
        if (attributeName.equalsIgnoreCase("linktext"))
            return By.linkText(attributeValue);
        if (attributeName.equalsIgnoreCase("name"))
            return By.name(attributeValue);
        if (attributeName.equalsIgnoreCase("partialLinkText"))
            return By.partialLinkText(attributeValue);
        return By.xpath("//*[@" + attributeName + "='" + attributeValue + "']");

    }

}
