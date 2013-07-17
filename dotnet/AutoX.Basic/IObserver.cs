#region

using System.Xml.Linq;

#endregion

namespace AutoX.Basic
{
    public interface IObserver
    {
        void Update(XElement changes);
    }
}