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
        private static readonly IData Data = null;
        //return proper IData according the config (MongoDB or mysql)
        public static IData GetData()
        {
            if (Data != null)
                return Data;
            if (Configuration.HasKey("PostgreSQLDBConnectionString"))
                return new PostgreSQLData();
            if (Configuration.HasKey("MySQLDBConnectionString"))
                return new MysqlData();
            return Configuration.HasKey("MongoDBConnectionString") ? new MongoData() : null;
        }
    }

   
}