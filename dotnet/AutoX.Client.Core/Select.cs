using System.Xml.Linq;
using OpenQA.Selenium.Support.UI;

namespace AutoX.Client.Core
{
    internal class Select : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            if (UIObject == null || UIObject.Count == 0)
            {
                sr.Error("Expected UI Object is not found!");
            }
            else
            {
                var select = new SelectElement(UIObject[0]);
                if (!string.IsNullOrEmpty(Data))
                {
                    select.SelectByText(Data);
                    //select.FindElement(By.CssSelector("option[value='3']")).Selected;
                }
            }
            return sr.GetResult();
        }
    }
}
