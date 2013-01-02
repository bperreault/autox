// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    public class Command : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            Browser.DosCommand(Data);
            return sr.GetResult();
        }
    }
}