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
            _result.SetAttributeValue(Constants._ID, Guid.NewGuid().ToString());
            _result.SetAttributeValue(Constants.ACTION, action.GetType().Name);
            _result.SetAttributeValue(Constants.DATA, action.Data);
            Success();
        }

        private void Success()
        {
            if (_result.Attribute(Constants.RESULT) == null)
            {
                _result.SetAttributeValue(Constants.RESULT, "Success");
            }
        }

        public void Error(string reason)
        {
            _result.SetAttributeValue(Constants.RESULT, "Error");
            SetReason(reason);
        }

        public void Warning(string reason)
        {
            var xResult = _result.Attribute(Constants.RESULT);
            if (xResult == null)
                _result.SetAttributeValue(Constants.RESULT, "Warning");
            else
            {
                var value = xResult.Value;
                if (!value.Equals("Error"))
                    _result.SetAttributeValue(Constants.RESULT, "Warning");
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