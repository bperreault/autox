using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoX.Basic;

namespace AutoX.WF.Core
{
    public class ActionsFactory
    {
        public static XElement Handle(string commandString)
        {
            var command = XElement.Parse(commandString);
            var action = GetActionName(command);
            //reflection way to get the class, then call it, get result
            if (!action.Contains("."))
                action = "AutoX.WF.Core.Actions." + action;
            var act = Type.GetType(action);
            if (act != null)
            {
                dynamic actDyn = Activator.CreateInstance(act);
                return actDyn.Do(command);
            }
            var r = (@"<Result Result='Failed' Reason='Encounter UnKnown Action[" + action + "]' />");
            return XElement.Parse(r);
            
        }
        private static string GetActionName(XElement command)
        {
            return command.GetAttributeValue("Action");
        }
    }
}
