using AutoX.Basic;
using System.Xml.Linq;

namespace AutoX.WF.Core.Actions
{
    internal interface IAction
    {
        XElement Do(XElement action);
    }

    internal class DeleteInstance : IAction
    {
        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }

    //class GetById : IAction
    //{
    //    public XElement Do(XElement action)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
    //class GetChildren : IAction
    //{
    //    public XElement Do(XElement action)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
    internal class GetComputersInfo : IAction
    {
        public XElement Do(XElement action)
        {
            return ClientInstancesManager.GetInstance().ToXElement();
        }
    }

    internal class GetInstancesInfo : IAction
    {
        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class Register : IAction
    {
        public XElement Do(XElement action)
        {
            ClientInstancesManager.GetInstance().Register(action);
            return XElement.Parse("<Result Result='Success' />");
        }
    }

    internal class RequestCommand : IAction
    {
        public XElement Do(XElement action)
        {
            string clientInstanceId = action.Attribute(Constants._ID).Value;
            return ClientInstancesManager.GetInstance().GetComputer(clientInstanceId).GetCommand();
        }
    }

    //class SetById : IAction
    //{
    //    public XElement Do(XElement action)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
    internal class SetInstanceInfo : IAction
    {
        public XElement Do(XElement action)
        {
            if (InstanceManager.GetInstance().UpdateInstance(action)) return XElement.Parse("<Result Result='Success' />");
            else return XElement.Parse("<Result Result='Error' />");
        }
    }

    internal class SetResult : IAction
    {
        public XElement Do(XElement action)
        {
            return InstanceManager.GetInstance().SetResult(action);           
        }
    }

    internal class StartInstance : IAction
    {
        public XElement Do(XElement action)
        {
            return InstanceManager.GetInstance().StartInstance(action); 
        }
    }

    internal class StopInstance : IAction
    {
        public XElement Do(XElement action)
        {
            return InstanceManager.GetInstance().StopInstance(action);
        }
    }
}