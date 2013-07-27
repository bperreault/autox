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
        private static IData _data = null;
        //return proper IData according the config (MongoDB or mysql)
        public static IData GetData()
        {
            if (_data != null)
                return _data;
            if (Configuration.HasKey("PostgreSQLDBConnectionString"))
                return new PostgreSQLData();
            if (Configuration.HasKey("MySQLDBConnectionString"))
                return new MysqlData();
            if(Configuration.HasKey("MongoDBConnectionString"))
                return new MongoData();
            
            return null;
        }
    }

   
}