#region

using System;
using AutoX.Basic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

#endregion

namespace AutoX.DB
{
    public class DBManager
    {
        private static DBManager _instance;


        private readonly MongoDatabase _database;

        private DBManager()
        {
            var connectionString = Configuration.Settings("DBConnectionString", "mongodb://localhost");

            var server = MongoServer.Create(connectionString);
            server.Connect();
            _database = server.GetDatabase(Configuration.Settings("DBName", "Automation"));
        }

        public static DBManager GetInstance()
        {
            if (_instance == null)
                _instance = new DBManager();
            return _instance;
        }

        public string find(Type type, string id)
        {
            var query = Query.EQ("_id", id);
            MongoCursor cursor = _database.GetCollection(type.Name).Find(query);

            if (cursor.Count() == 0)
                return null;
            return cursor.ToString();
        }
    }
}