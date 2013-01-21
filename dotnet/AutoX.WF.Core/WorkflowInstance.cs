using System.IO;
using AutoX.Activities;
using AutoX.Activities.AutoActivities;
using AutoX.Basic;
using System.Collections.Generic;
using AutoX.DB;
using System.Xml.Linq;
using System.Threading;
using System.Activities;
using System.Activities.XamlIntegration;


namespace AutoX.WF.Core
{
    public class WorkflowInstance : IHost,IObserable
    {
        private Dictionary<string,string > _variables = new Dictionary<string, string>();
        private volatile XElement _command;
        private volatile XElement _result;
        private List<IObserver> _observers = new List<IObserver>();
        public WorkflowInstance(string workflowId, Dictionary<string, string> upperLevelVariables)
        {
            if(upperLevelVariables!=null)
                foreach (var upperLevelVariable in upperLevelVariables)
                {
                    _variables.Add(upperLevelVariable.Key,upperLevelVariable.Value);
                }
            XElement script = GetDataObject(workflowId);
            if(script!=null)
                StartActivity(script.GetAttributeValue("Content"));

        }
        
        private void StartActivity(string workflow)
        {
            var activity = ActivityXamlServices.Load(new StringReader(workflow)) as AutomationActivity;
            if (activity != null)
            {
                activity.SetHost(this);
                //var idleEvent = new AutoResetEvent(false);
                //TODO write log in workflow!!!

                var workflowApplication = Utilities.GetWorkflowApplication(activity);
                workflowApplication.Run();
            }
            
        }

        public XElement GetDataObject(string id)
        {
            return Data.Read(id);
        }

        public void SetCommand(XElement steps)
        {            
                _command = steps;
        }

        public XElement GetResult(string guid)
        {
            int count = 0;
            while (_result == null)
            {
                Thread.Sleep(1000);
                count++;
                if (count > 300)
                    return null;
            }
            string resultString = _result.ToString();
            _result = null;
            return XElement.Parse(resultString);
        }

        public void SetResult(XElement result)
        { 
                _result = result;
                Notify(result);
        }

        public XElement GetCommand()
        {
            int count = 0;
            while (_command == null)
            {
                Thread.Sleep(1000);
                count++;
                if (count > 300)
                    return null;
            }
            string commandString = _command.ToString();
            _command = null;
            return XElement.Parse(commandString);
        }

        public void Register(IObserver observer)
        {
            if(!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Notify(XElement change)
        {
            foreach (var observer in _observers)
            {
                observer.Update(change);
            }
        }
    }
}
