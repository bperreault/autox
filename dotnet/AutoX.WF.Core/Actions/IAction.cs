
using System.Xml.Linq;

namespace AutoX.WF.Core.Actions
{
    interface IAction
    {
        XElement Do(XElement action);
    }

    class DeleteInstance : IAction
    {

        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }
    class GetById : IAction
    {

        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }
    class GetChildren : IAction
    {

        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }
    class GetComputersInfo : IAction
    {

        public XElement Do(XElement action)
        {
            return ClientInstancesManager.GetInstance().ToXElement();
        }
    }
    class GetInstancesInfo : IAction
    {

        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }
    class Register : IAction
    {

        public XElement Do(XElement action)
        {
            ClientInstancesManager.GetInstance().Register(action);
            return XElement.Parse("<Result Result='Success' />");
        }
    }
    class RequestCommand : IAction
    {

        public XElement Do(XElement action)
        {
            string clientInstanceId = action.Attribute("_id").Value;
            return ClientInstancesManager.GetInstance().GetComputer(clientInstanceId).GetCommand();
        }
    }
    class SetById : IAction
    {

        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }
    class SetInstanceInfo : IAction
    {

        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }
    class SetResult : IAction
    {

        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }
    class StartInstance : IAction
    {

        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }
    class StopInstance : IAction
    {
        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }
}
