using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoX.Basic
{
    public interface IObserable
    {
        void Register(IObserver observer);
        void Notify(XElement change);
    }
}
