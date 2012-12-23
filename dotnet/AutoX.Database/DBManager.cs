// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoX.Basic;
using AutoX.Basic.Model;

#endregion

namespace AutoX.Database
{
    public class DBManager
    {
        //put this string to your web.config, you can change this string to connect to another database
        private static readonly string DataBaseString =Configuration.ConnectionString();
        
        private static readonly DB AutoDB = new DB(DataBaseString);
        private static DBManager _instance;

        private DBManager()
        {
            
            if (!AutoDB.DatabaseExists())
            {
                AutoDB.CreateDatabase();

                //insert project, data, ui, result
                string projectId = Configuration.Settings("ProjectRoot", "0010010000001");
                string dataId = Configuration.Settings("DataRoot", "0030030000003");
                string uiId = Configuration.Settings("ObjectPool", "0040040000004");
                string resultId = Configuration.Settings("ResultsRoot", "0020020000002");
                //DataObjectExt.GetDataObjectFromXElement(@"<Project GUID='"+projectId+"' ParentId='' Type='Project' />");
                var project = new Folder {GUID = projectId, Name = "Project"};
                var data = new Folder {GUID = dataId, Name = "Data"};
                var ui = new Folder {GUID = uiId, Name = "UI"};
                var result = new Folder {GUID = resultId, Name = "Result"};
                AddOrUpdateOneDataToDB(projectId, "", project);
                AddOrUpdateOneDataToDB(dataId, "", data);
                AddOrUpdateOneDataToDB(uiId, "", ui);
                AddOrUpdateOneDataToDB(resultId, "", result);
            }
            //Load Cache here; is it a good idea???
            //var query = from o in AutoDB.Indexes
            //            select o;
            //foreach (var index in query)
            //{
            //    string GUID = index.GUID;
            //    string parentId = index.ParentId;
            //    _cache.TryAdd(GUID, parentId);
            //}
        }

        public static DBManager GetInstance()
        {
            return _instance ?? (_instance = new DBManager());
        }

        public List<IDataObject> FindDataFromDB(string parentId)
        {
            var list = new List<IDataObject>();
            IQueryable<Index> query = from o in AutoDB.Indexes
                                      where o.ParentId.Equals(parentId)
                                      select o;
            foreach (Index index in query)
            {
                string guid = index.GUID;
                string type = index.Type;
                ITable table = GetTable(type);
                if (table == null)
                    continue;
                var tQueryable = table as IQueryable<IDataObject>;
                //if (tQueryable == null)
                //    continue;
                if (tQueryable.Count(c => c.GUID.Equals(guid)) == 0)
                {
                    continue;
                }
                IDataObject dataObject = tQueryable.First(c => c.GUID.Equals(guid));
                if (dataObject != null) list.Add(dataObject);
            }
            return list;
        }

        public ITable GetTable(string type)
        {
            if (type.Equals("Data"))
                return AutoDB.Data;
            if (type.Equals("Datum"))
                return AutoDB.Data;
            if (type.Equals("Folder"))
                return AutoDB.Folders;
            if (type.Equals("Result"))
                return AutoDB.Results;
            if (type.Equals("Script"))
                return AutoDB.Scripts;
            if (type.Equals("UIObject"))
                return AutoDB.UIPool;
            if (type.Equals("Translation"))
                return AutoDB.Translations;
            return null;
        }

        public IDataObject FindOneDataFromDB(string GUID)
        {
            IQueryable<Index> query = from o in AutoDB.Indexes
                                      where o.GUID.Equals(GUID)
                                      select o;
            if (!query.Any())
                return null;
            string type = query.First().Type;
            ITable table = GetTable(type);
            if (table == null)
                return null;
            var tQueryable = table as IQueryable<IDataObject>;
            if (tQueryable.Count(c => c.GUID.Equals(GUID)) == 0)
            {
                return null;
            }
            return tQueryable.First(c => c.GUID.Equals(GUID));
        }

        public void DeleteOneDataFromDB(string guid)
        {
            IQueryable<Index> q = from o in AutoDB.Indexes
                                  where o.GUID.Equals(guid)
                                  select o;
            Index itself = q.First();
            if (itself == null)
                return;
            //recursive deletion   
            // delete all its children
            IQueryable<Index> query = from o in AutoDB.Indexes
                                      where o.ParentId.Equals(guid)
                                      select o;
            foreach (Index index in query)
            {
                DeleteOneDataFromDB(index.GUID);
            }
            // then delete itself
            string type = itself.Type;
            AutoDB.Indexes.DeleteOnSubmit(itself);
            DeleteOneItemInEntityTableFromDB(guid, type);
            AutoDB.SubmitChanges();
        }

        public void DeleteOneItemInEntityTableFromDB(string guid, string type)
        {
            ITable table = GetTable(type);
            if (table == null)
                return;
            var tQueryable = table as IQueryable<IDataObject>;
            if (tQueryable.Count(c => c.GUID.Equals(guid)) == 0)
            {
                return;
            }
            IDataObject data = tQueryable.First(c => c.GUID.Equals(guid));
            if (data != null)
            {
                table.DeleteOnSubmit(data);
                AutoDB.SubmitChanges();
            }
        }

        public void AddOrUpdateOneDataToDB(string guid, string parentId, IDataObject iDataObject)
        {
            //check 2 places: 1. index 2. entity table
            //if existed, update it.
            string type = iDataObject.GetType().Name;
            var index = new Index
                            {
                                GUID = guid,
                                ParentId = parentId,
                                Type = type,
                                Created = DateTime.UtcNow,
                                Updated = DateTime.UtcNow
                            };
            IQueryable<Index> q = from o in AutoDB.Indexes
                                  where o.GUID.Equals(guid)
                                  select o;
            if (iDataObject.Created <= DateTime.MinValue)
                iDataObject.Created = DateTime.UtcNow;
            iDataObject.Updated = DateTime.UtcNow;
            if (!q.Any())
            {
                iDataObject.Created = DateTime.UtcNow;
                AutoDB.Indexes.InsertOnSubmit(index);
                AutoDB.GetTable(iDataObject.GetType()).InsertOnSubmit(iDataObject);
                AutoDB.SubmitChanges();
            }
            else
            {
                AutoDB.Indexes.DeleteOnSubmit(q.First());
                AutoDB.Indexes.InsertOnSubmit(index);
                ITable table = GetTable(type);
                if (table == null)
                {
                    Logger.GetInstance().Log().Error("Type[" + type + "] of table does not existed!");
                    return;
                }
                AddOrUpdateEntity(guid, iDataObject, table);

                AutoDB.SubmitChanges();
            }
        }

        private static void AddOrUpdateEntity(string guid, IDataObject iDataObject, ITable table)
        {
            var tQueryable = table as IQueryable<IDataObject>;
            iDataObject.Updated = DateTime.UtcNow;
            if (tQueryable.Count(c => c.GUID.Equals(guid)) == 0)
            {
                iDataObject.Created = DateTime.UtcNow;
                table.InsertOnSubmit(iDataObject);
            }
            else
            {
                IDataObject data = tQueryable.First(c => c.GUID.Equals(guid));
                if (data == null)
                {
                    //not existed, need to insert
                    table.InsertOnSubmit(iDataObject);
                }
                else
                {
                    if (data.Created != null)
                        iDataObject.Created = data.Created;

                    table.DeleteOnSubmit(data);

                    table.InsertOnSubmit(iDataObject);
                }
            }
        }
    }
}