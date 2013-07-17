#region

using System.Xml.Linq;
using AutoX.Basic;

#endregion

namespace AutoX.DB
{
    public interface IData
    {
        bool Save(XElement data);
        XElement Read(string id);
        //XElement Read(string key, string value);
        void Delete(string id);
        XElement GetChildren(string id);
    }

    public static class DBFactory
    {
        private static IData _data;
        //return proper IData according the config (MongoDB or mysql)
        public static IData GetData()
        {
            if (_data != null)
                return _data;
            var connectionString = Configuration.Settings("DBConnectionString",
                "Server=localhost;Database=autox;Uid=root;Pwd=Gene4hje;");
            if (connectionString.Contains("mongo"))
                _data = new MongoData();
            else
                _data = new MysqlData();
            return _data;
        }
    }
}