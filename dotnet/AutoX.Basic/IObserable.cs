#region

using System.Xml.Linq;

#endregion

namespace AutoX.Basic
{
    public interface IObserable
    {
        void Register(IObserver observer);

        void Notify(XElement change);
    }
}