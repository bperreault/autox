#region

using System;
using System.Threading;
using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    public class Wait : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            var time = 17;
            if (!string.IsNullOrEmpty(Data))
            {
                time = Convert.ToInt32(Data);
            }
            Thread.Sleep(time*1000);
            return sr.GetResult();
        }
    }
}