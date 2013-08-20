using System.Linq;
using System.Xml.Linq;
using OpenQA.Selenium;

namespace AutoX.Client.Core
{
    internal class VerifyTable : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            if (UIObject == null || UIObject.Count == 0)
            {
                sr.Error("Expected Table is not found!");
            }
            else
            {
                var target = UIObject[0];
                var rows = target.FindElements(By.TagName("tr"));
                if (rows.Any(VerifyRowMatched))
                {
                    return sr.GetResult();
                }
                sr.Error("Expected Row not found, Data["+Data+"]");
                
            }
            return sr.GetResult();
        }

        private bool VerifyRowMatched(ISearchContext webElement)
        {
            var values = Data.Split('|');
            return values.Select(value => webElement.FindElement(By.XPath("//*[contains(text(),'" + value + "')]"))).All(td => td != null);
        }
    }
}
