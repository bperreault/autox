// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

namespace AutoX.Client.Core
{
    public class StepResult
    {
        private readonly XElement _result = new XElement("StepResult");

        public StepResult(AbstractAction action)
        {
            
            _result.SetAttributeValue("_id",Guid.NewGuid().ToString());
            _result.SetAttributeValue("Action", action.GetType().Name);
            _result.SetAttributeValue("Data", action.Data);
            Success();
        }

        private void Success()
        {
            if (_result.Attribute("Result") == null)
            {
                _result.SetAttributeValue("Result", "Success");
            }
        }

        public void Error(string reason)
        {
            _result.SetAttributeValue("Result", "Error");
            SetReason(reason);
        }

        public void Warning(string reason)
        {
            var xResult = _result.Attribute("Result");
            if (xResult == null)
                _result.SetAttributeValue("Result", "Warning");
            else
            {
                var value = xResult.Value;
                if (!value.Equals("Error"))
                    _result.SetAttributeValue("Result", "Warning");
            }
            SetReason(reason);
        }

        public void SetReason(string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                return;
            }

            var xReason = _result.Attribute("Reason");
            if (xReason == null)
                _result.SetAttributeValue("Reason", reason);
            else
            {
                _result.SetAttributeValue("Reason", xReason.Value + "\n" + reason);
            }
        }

        public XElement GetResult()
        {
            return _result;
        }
    }
}