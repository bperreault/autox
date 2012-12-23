// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace AutoX.Basic.Model
{
    public class TableItem : IEnumerable
    {
        private ObservableCollection<object> _list = new ObservableCollection<object>();

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        public ObservableCollection<object> Get()
        {
            return _list;
        }

        public void Set(ObservableCollection<object> value)
        {
            _list = value ?? new ObservableCollection<object>();
        }

        public void Add(object row)
        {
            _list.Add(row);
        }

        public void Remove(object row)
        {
            _list.Remove(row);
        }

        public ObservableCollection<object> GetMatched(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return _list;
            var ret = new ObservableCollection<object>();
            foreach (
                object rowItem in _list.Where(rowItem => rowItem.GetXElementFromObject().ToString().Contains(filter)))
            {
                ret.Add(rowItem);
            }
            return ret;
        }

        public void Clear()
        {
            _list = new ObservableCollection<object>();
        }
    }
}