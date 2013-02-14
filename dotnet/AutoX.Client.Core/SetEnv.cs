using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
