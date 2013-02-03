using System.Xml.Linq;

namespace AutoX.Basic
{
    public interface IObserable
    {
        void Register(IObserver observer);

        void Notify(XElement change);
    }
}