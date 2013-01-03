
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
    }
    class RequestCommand : IAction
    {

        public XElement Do(XElement action)
        {
            throw new System.NotImplementedException();
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
