#region

using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    public class Close : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            Browser.CloseBrowser();
            return sr.GetResult();
        }
    }
}