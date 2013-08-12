using System.Xml.Linq;

namespace AutoX.Client.Core
{
    internal class NotExisted: AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            if (UIObject == null || UIObject.Count == 0)
            {
                
            }
            else
            {
                sr.Error("Unexpected UI Object is found!");
            }
            return sr.GetResult();
        }
    }
    
}
