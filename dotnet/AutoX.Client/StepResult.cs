// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

namespace AutoX.Client
{
    public class StepResult
    {
        private readonly XElement _result = new XElement("StepResult");

        public StepResult(AbstractAction action)
        {
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

            SnapOn("SnapshotOnError");
        }

        private void SnapOn(string config)
        {
            string snapshotOnError = Configuration.Settings(config, "false");
            if (Convert.ToBoolean(snapshotOnError))
            {
                string content = Browser.GetInstance().Snapshot();
                var snap = new XElement("Snapshot");
                snap.SetAttributeValue("Snapshot", content);
                _result.Add(snap);
            }
        }

        public void Warning(string reason)
        {
            XAttribute xResult = _result.Attribute("Result");
            if (xResult == null)
                _result.SetAttributeValue("Result", "Warning");
            else
            {
                string value = xResult.Value;
                if (!value.Equals("Error"))
                    _result.SetAttributeValue("Result", "Warning");
            }
            SetReason(reason);
            SnapOn("SnapshotOnWarning");
        }

        public void SetReason(string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                return;
            }

            XAttribute xReason = _result.Attribute("Reason");
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