#region

using System;
using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    internal class Enter : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            if (UIObject == null||UIObject.Count == 0)
            {
                sr.Error("Expected UI Object is not found!");
            }
            else
            {
                var text = UIObject[0];
                try
                {
                    text.Clear();
                }
                catch (Exception ex)
                {
                    //some objects cannot be cleared, like hidden, file.upload, so ignore this error
                    if (!string.IsNullOrEmpty(XPath) && !string.IsNullOrEmpty(Data))
                    {
                        Browser.ExecuteJavaScript("document.evaluate('" + XPath.Replace("'", "\\\"") + "',document,null,9,null).singleNodeValue.value = '" + Data + "';");
                        return sr.GetResult();
                    }
                }
                if (!string.IsNullOrEmpty(Data))
                {
                    
                    text.SendKeys(Data);
                }
            }
            return sr.GetResult();
        }
    }
}