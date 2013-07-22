#region

using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    internal class Click : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            if (UIObject.Count == 0)
            {
                sr.Error("Expected UI Object is not found!");
            }
            else
            {
                UIObject[0].Click();
            }
            return sr.GetResult();
        }
    }
}