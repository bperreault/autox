#region

using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    public class Start : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            Browser.GetCurrentBrowser();
            return sr.GetResult();
        }
    }
}