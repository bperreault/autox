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
        private MongoCollection project;

        private DBManager()
        {
            string userName = Configuration.Settings("UserName", "jien.huang");
            string productId = AsymmetricEncryption.GetProductId();
            var connectionString = Configuration.Settings("DBConnectionString", "mongodb://"+userName+":"+productId+"@localhost"); //mongodb://uname:pwd@localhost
            var client = new MongoClient(connectionString);
            var server = client.GetServer(); //MongoServer.Create(connectionString);
            server.Connect();
            _database = server.GetDatabase(Configuration.Settings("DBName", "autox"));
            project = _database.GetCollection(Configuration.Settings("ProjectName", "Data"));
        }

        

        public static DBManager GetInstance()
        {
            return _instance ?? (_instance = new DBManager());
        }

        public bool AddUser(string newUser, string newPassword)
        {
            if (_database.FindUser(newUser) == null)
            {
                var user = new MongoCredentials(newUser, newPassword, false);
                _database.AddUser(user);
                return true;
            }
            return false;
        }

        public bool IsProjectExisted(string projectName)
        {
            return _database.GetCollection(projectName).Count() > 0;
        }

        public void SetProject(string projectName)
        {
            project = _database.GetCollection(projectName);
        }

        public BsonDocument Find(string id)
        {
            return project.FindOneByIdAs<BsonDocument>(id);
        }
        public bool Save(BsonDocument bsonDocument)
        {
            return project.Save(bsonDocument).Ok;
        }
        public void Delete(string id)
        {
            var query = Query.EQ("_id", id);
            project.Remove(query);
        }
    }
}