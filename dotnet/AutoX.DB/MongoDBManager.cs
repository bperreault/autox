#region

using System;
using System.Collections.Generic;
using AutoX.Basic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

#endregion

namespace AutoX.DB
{
    public class MongoDBManager
    {
        private static MongoDBManager _instance;

        private readonly MongoDatabase _database;
        private MongoCollection project;

        private MongoDBManager()
        {
            try
            {
                var userName = Configuration.Settings("UserName", "jien.huang");
                var productId = AsymmetricEncryption.GetProductId();
                var connectionString = Configuration.Settings("DBConnectionString", "mongodb://@localhost");
                //mongodb://uname:pwd@localhost

                //var connectionString = Configuration.Settings("DBConnectionString", "mongodb://" + userName + ":" + productId + "@localhost"); //mongodb://uname:pwd@localhost
                var client = new MongoClient(connectionString);


                var server = client.GetServer(); //MongoServer.Create(connectionString);
                //server.Connect();
                _database = server.GetDatabase(Configuration.Settings("DBName", "autox"));
                project = _database.GetCollection(Configuration.Settings("ProjectName", Constants.DATA));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static MongoDBManager GetInstance()
        {
            return _instance ?? (_instance = new MongoDBManager());
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
            return
                project.FindOneAs<BsonDocument>(Query.And(Query.EQ(Constants._ID, id), Query.Exists(Constants.PARENT_ID),
                    Query.NE(Constants.PARENT_ID, "Deleted")));

            //             project.FindOneByIdAs<BsonDocument>(id);
        }

        public BsonDocument Find(string key, string value)
        {
            return project.FindOneAs<BsonDocument>(Query.EQ(key, value));
        }

        public List<BsonDocument> Kids(string parentId)
        {
            var children = new List<BsonDocument>();
            var sort = SortBy.Ascending("Created");
            MongoCursor cursor = project.FindAs<BsonDocument>(Query.EQ(Constants.PARENT_ID, parentId))
                .SetSortOrder(sort);
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
            var kidQuery = Query.EQ(Constants.PARENT_ID, id);
            MongoCursor kidCursor = project.FindAs<BsonDocument>(kidQuery);
            foreach (BsonDocument kid in kidCursor)
            {
                Delete(kid.GetValue(Constants._ID).ToString());
            }
            var query = Query.EQ(Constants._ID, id);
            project.Remove(query);
        }
    }
}