#region

using System;
using AutoX.Basic;
using MongoDB.Bson;
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
            string userName = Configuration.Settings("UserName", "jien.huang");
            string productId = AsymmetricEncryption.GetProductId();
            var connectionString = Configuration.Settings("DBConnectionString", "mongodb://"+userName+":"+productId+"@localhost"); //mongodb://uname:pwd@localhost
            var client = new MongoClient(connectionString);
            var server = client.GetServer(); //MongoServer.Create(connectionString);
            server.Connect();
            _database = server.GetDatabase(Configuration.Settings("DBName", "autox"));
        }

        public static DBManager GetInstance()
        {
            return _instance ?? (_instance = new DBManager());
        }

        public BsonDocument Find(string id)
        {
            return _database.GetCollection("Data").FindOneByIdAs<BsonDocument>(id);
        }
        public bool Save(BsonDocument bsonDocument)
        {
            return _database.GetCollection("Data").Save(bsonDocument).Ok;
        }
        public void Delete(string id)
        {
            var query = Query.EQ("_id", id);
            _database.GetCollection("Data").Remove(query);
        }
    }
}