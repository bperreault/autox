﻿// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Xml.Linq;

#endregion

namespace AutoX.Client
{
    public class Start : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            Browser.GetInstance().GetCurrentBrowser();
            return sr.GetResult();
        }
    }
}