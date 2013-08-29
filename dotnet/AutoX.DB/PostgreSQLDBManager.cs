using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using AutoX.Basic;
using Npgsql;

namespace AutoX.DB
{
    public class PostgreSQLDBManager
    {
        private static PostgreSQLDBManager _instance;
        private readonly NpgsqlConnection _connection;

        private PostgreSQLDBManager()
        {
            _connection = new NpgsqlConnection(Configuration.Settings("PostgreSQLDBConnectionString", 
                "User ID=root;Password=Passw0rd;Host=localhost;Port=5432;Database=autox;Pooling=true;"));
            try
            {
                _connection.Open();
            }
            catch (Exception exception)
            {
                Log.Error(ExceptionHelper.FormatStackTrace("Connect to PostgreSQL failed.",exception));
                return;
            }
            
            var cmd = new NpgsqlCommand("select count(*) from content", _connection);
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count > 0)
                return;
            var rootId = Configuration.Settings("Root", "42c5eb51-0e1c-4de1-976d-733bde24220a");
            CreateSubItem(rootId, "Folder", "Data");
            CreateSubItem(rootId, "Folder", "UI");
            CreateSubItem(rootId, "Folder", "Translation");
            CreateSubItem(rootId, "Folder", "Project");
            CreateSubItem(rootId, "Folder", "Result");

            CreateSubItem("", "Project", "Root", rootId);
        }

        private void CreateSubItem(string parentId, string type, string name)
        {
            var id = Guid.NewGuid().ToString();
            CreateSubItem(parentId, type, name, id);
        }

        private void CreateSubItem(string parentId, string type, string name, string id)
        {
            var xElement = new XElement(type);
            xElement.SetAttributeValue(Constants._ID, id);
            xElement.SetAttributeValue(Constants.PARENT_ID, parentId);
            xElement.SetAttributeValue(Constants._TYPE, type);
            xElement.SetAttributeValue(Constants.NAME, name);
            xElement.SetAttributeValue("Created", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            xElement.SetAttributeValue("Updated", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            CreateContent(id, xElement.ToString(), type, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            CreateRelationship(parentId, "Parent-Kid", id, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
        }

        public static PostgreSQLDBManager GetInstance()
        {
            return _instance ?? (_instance = new PostgreSQLDBManager());
        }

        public void RemoveRelationship(string id)
        {
            var comm1 = new NpgsqlCommand("select master from relationship where slave=@id", _connection);
            comm1.Parameters.AddWithValue("@id", id);
            var reader = comm1.ExecuteReader();
            if (reader.Read())
            {
                var masterId = reader.GetString(0);
                DBFactory.DeleteMemcachedRelationship(masterId, id);
            }
            reader.Close();

            var comm2 = new NpgsqlCommand("delete from relationship where slave=@id", _connection);
            comm2.Parameters.AddWithValue("@id", id);
            comm2.ExecuteNonQuery();
            
        }

        public void Remove(string id)
        {
            var comm1 = new NpgsqlCommand("delete from content where id=@id", _connection);
            comm1.Parameters.AddWithValue("@id", id);
            comm1.ExecuteNonQuery();
            DBFactory.DeleteMemcachedData(id);

            //var comm2 = new NpgsqlCommand("delete from relationship where slave=@id", _connection);
            //comm2.Parameters.AddWithValue("@id", id);
            //comm2.ExecuteNonQuery();
            RemoveRelationship(id);
            
            var list = DBFactory.GetMemcachedRelationship(id);
            if (list == null)
            {
                list = new List<string>();
                var comm3 = new NpgsqlCommand("select slave from relationship where master=@id", _connection);
                comm3.Parameters.AddWithValue("@id", id);
                var reader = comm3.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));

                }
                reader.Close();
            }

            
            foreach (var kidId in list)
            {
                Remove(kidId);
            }

            var comm4 = new NpgsqlCommand("delete from relationship where master=@id", _connection);
            comm4.Parameters.AddWithValue("@id", id);
            comm4.ExecuteNonQuery();
            DBFactory.DeleteMemcachedRelationship(id);
        }

        public XElement Find(string id)
        {
            string content = DBFactory.GetMemcachedData(id);
            if (!string.IsNullOrEmpty(content))
                return XElement.Parse(content);
            var cmd = new NpgsqlCommand("select data from content where id=@id", _connection);
            cmd.Parameters.AddWithValue("@id", id);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                content = reader.GetString(0);
            }
            reader.Close();
            if (content == null)
                return null;
            DBFactory.UpdateMemcachedData(id, content);
            return XElement.Parse(content);
        }

        private IEnumerable<string> GetKids(string parentId)
        {
            var list = new List<string>();
            var cmd = new NpgsqlCommand("select slave from relationship where master=@parentId and type='Parent-Kid' order by created",
                _connection);
            cmd.Parameters.AddWithValue("@parentId", parentId);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(reader.GetString(0));
            reader.Close();
            return list;
        }

        public XElement GetChildren(string parentId)
        {
            var kids = new XElement("Children");

            var kidsList = DBFactory.GetMemcachedRelationship(parentId);
            if (kidsList != null&& kidsList.Count>0)
            {
                foreach (var directKid in kidsList)
                {
                    kids.Add(Find(directKid));
                }
                return kids;
            }
            
            var directKids = GetKids(parentId);
            foreach (var directKid in directKids)
            {
                DBFactory.AddMemcachedRelationship(parentId, directKid);
                kids.Add(Find(directKid));
            }
            return kids;
        }

        public void UpdateContent(string id, string content,string updated)
        {
            var comm1 = new NpgsqlCommand("update content set data=@content, updated=@updated  where id=@id", _connection);
            comm1.Parameters.AddWithValue("@id", id);
            comm1.Parameters.AddWithValue("@content", content);
            comm1.Parameters.AddWithValue("@updated", updated);
            comm1.ExecuteNonQuery();
            DBFactory.UpdateMemcachedData(id, content);
        }

        //public void UpdateRelationship(string masterId, string type, string slaveId)
        //{
        //    var comm1 = new NpgsqlCommand("update relationship set type=@type, slave=@slave where master=@master",
        //        _connection);
        //    comm1.Parameters.AddWithValue("@master", masterId);
        //    comm1.Parameters.AddWithValue("@type", type);
        //    comm1.Parameters.AddWithValue("@slave", slaveId);
        //    comm1.ExecuteNonQuery();
        //}

        public void CreateContent(string id, string content,string type,string created,string updated)
        {
            try
            {
                var comm1 = new NpgsqlCommand("insert into content values(@id,@content,@type,@created,@updated)", _connection);
                comm1.Parameters.AddWithValue("@id", id);
                comm1.Parameters.AddWithValue("@content", content);
                comm1.Parameters.AddWithValue("@type", type);
                comm1.Parameters.AddWithValue("@created", created);
                comm1.Parameters.AddWithValue("@updated", updated);
                comm1.ExecuteNonQuery();
                DBFactory.UpdateMemcachedData(id, content);
            }
            catch (Exception ex)
            {
                Log.Error(ExceptionHelper.FormatStackTrace(ex));
            }
        }

        public void CreateRelationship(string master, string type, string slave, string created, string updated)
        {
            var comm1 = new NpgsqlCommand("insert into relationship values(@master,@type,@slave,@created,@updated)", _connection);
            comm1.Parameters.AddWithValue("@master", master);
            comm1.Parameters.AddWithValue("@type", type);
            comm1.Parameters.AddWithValue("@slave", slave);
            comm1.Parameters.AddWithValue("@created", created);
            comm1.Parameters.AddWithValue("@updated", updated);
            comm1.ExecuteNonQuery();
            DBFactory.AddMemcachedRelationship(master, slave);
        }
    }
}
