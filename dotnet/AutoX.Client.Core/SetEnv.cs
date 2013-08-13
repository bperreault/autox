#region

using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    internal class SetEnv : AbstractAction
    {
        public override XElement Act()
        {
            
            var sr = new StepResult(this);

            return sr.GetResult();
        }
    }
}