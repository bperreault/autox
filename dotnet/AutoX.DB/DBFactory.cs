using AutoX.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
    public class DBFactory
    {
        private static IData data;
        //return proper IData according the config (MongoDB or mysql)
        public static IData GetData()
        {
            if (data != null)
                return data;
            string connectionString = Configuration.Settings("DBConnectionString", "Server=localhost;Database=autox;Uid=root;Pwd=Gene4hje;");
            if(connectionString.Contains("mongo"))
                data = new MongoData();
            else 
                data = new MysqlData();
            return data;
        }
    }
}
