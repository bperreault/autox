using System.Xml.Linq;

namespace AutoX.Basic
{
    public interface IObserver
    {
        void Update(XElement changes);
    }
}