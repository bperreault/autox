// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Xml.Linq;

#endregion

namespace AutoX.Client
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