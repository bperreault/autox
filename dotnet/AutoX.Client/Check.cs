// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Xml.Linq;

#endregion

namespace AutoX.Client
{
    public class Check : AbstractAction
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
                if (string.IsNullOrEmpty(Data))
                    UIObject[0].Click();
                else
                {
                    bool toCheck = Convert.ToBoolean(Data);
                    bool checkStatus = UIObject[0].Selected;
                    if (toCheck && !checkStatus || !toCheck && checkStatus)
                        UIObject[0].Click();
                }
            }
            return sr.GetResult();
        }
    }
}