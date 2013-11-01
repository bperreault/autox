#region

using System.Threading;
using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    internal class Click : AbstractAction
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
                UIObject[0].Click();
                Thread.Sleep(500);
                Browser.DismissUnexpectedAlert();
            }
            return sr.GetResult();
        }
    }
}