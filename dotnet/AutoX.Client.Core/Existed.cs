using System.Xml.Linq;

namespace AutoX.Client.Core
{
    internal class Existed : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            if (UIObject == null || UIObject.Count == 0)
            {
                sr.Error("Expected UI Object is not found!");
            }
            
            return sr.GetResult();
        }
    }
}
