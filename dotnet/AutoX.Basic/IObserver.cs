using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoX.Basic
{
    public interface IObserver
    {
        void Update(XElement changes);
    }
}
