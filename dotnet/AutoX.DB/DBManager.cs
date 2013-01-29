#region

using System;
using System.Collections.Generic;
using AutoX.Basic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections;

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
            var connectionString = Configuration.Settings("DBConnectionString", "mongodb://huangjien:Weilian2@localhost"); //mongodb://uname:pwd@localhost
            //var connectionString = Configuration.Settings("DBConnectionString", "mongodb://" + userName + ":" + productId + "@localhost"); //mongodb://uname:pwd@localhost
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

            return project.FindOneAs<BsonDocument>(Query.And(Query.EQ("_id", id), Query.Exists("_parentId"), Query.NE("_parentId", "Deleted")));
//             project.FindOneByIdAs<BsonDocument>(id);
        }

        public BsonDocument Find(string key, string value)
        {
            return project.FindOneAs<BsonDocument>(Query.EQ(key, value));
        }
        public List<BsonDocument> Kids(string parentId)
        {
            var children = new List<BsonDocument>();
            MongoCursor cursor = project.FindAs<BsonDocument>(Query.EQ("_parentId", parentId));
            foreach (BsonDocument variable in cursor)
            {
                children.Add(variable);
            }
            return children;
        } 
        public bool Save(BsonDocument bsonDocument)
        {
            return project.Save(bsonDocument).Ok;
        }
        public void Delete(string id)
        {
            if (id == null)
                return;
            var kidQuery = Query.EQ("_parentId", id);
            MongoCursor kidCursor = project.FindAs<BsonDocument>(kidQuery);
            foreach (BsonDocument kid in kidCursor)
            {
                Delete(kid.GetValue("_id").ToString());
            }
            var query = Query.EQ("_id", id);
            project.Remove(query);
        }
    }
}