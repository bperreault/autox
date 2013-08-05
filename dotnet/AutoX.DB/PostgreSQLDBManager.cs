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
            _connection.Open();
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
            CreateContent(id, xElement.ToString());
            CreateRelationship(parentId, "Parent-Kid", id);
        }

        public static PostgreSQLDBManager GetInstance()
        {
            return _instance ?? (_instance = new PostgreSQLDBManager());
        }

        public void RemoveRelationship(string id)
        {
            var comm2 = new NpgsqlCommand("delete from relationship where slave=@id", _connection);
            comm2.Parameters.AddWithValue("@id", id);
            comm2.ExecuteNonQuery();
        }

        public void Remove(string id)
        {
            var comm1 = new NpgsqlCommand("delete from content where id=@id", _connection);
            comm1.Parameters.AddWithValue("@id", id);
            comm1.ExecuteNonQuery();
            var comm2 = new NpgsqlCommand("delete from relationship where master=@id or slave=@id", _connection);
            comm2.Parameters.AddWithValue("@id", id);
            comm2.ExecuteNonQuery();
        }

        public XElement Find(string id)
        {
            string content = null;
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
            return XElement.Parse(content);
        }

        private IEnumerable<string> GetKids(string parentId)
        {
            var list = new List<string>();
            var cmd = new NpgsqlCommand("select slave from relationship where master=@parentId and type='Parent-Kid'",
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
            var directKids = GetKids(parentId);
            foreach (string directKid in directKids)
            {
                kids.Add(Find(directKid));
            }
            return kids;
        }

        public void UpdateContent(string id, string content)
        {
            var comm1 = new NpgsqlCommand("update content set data=@content  where id=@id", _connection);
            comm1.Parameters.AddWithValue("@id", id);
            comm1.Parameters.AddWithValue("@content", content);
            comm1.ExecuteNonQuery();
        }

        public void UpdateRelationship(string masterId, string type, string slaveId)
        {
            var comm1 = new NpgsqlCommand("update relationship set type=@type, slave=@slave where master=@master",
                _connection);
            comm1.Parameters.AddWithValue("@master", masterId);
            comm1.Parameters.AddWithValue("@type", type);
            comm1.Parameters.AddWithValue("@slave", slaveId);
            comm1.ExecuteNonQuery();
        }

        public void CreateContent(string id, string content)
        {
            try
            {
                var comm1 = new NpgsqlCommand("insert into content values(@id,@content)", _connection);
                comm1.Parameters.AddWithValue("@id", id);
                comm1.Parameters.AddWithValue("@content", content);
                comm1.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CreateRelationship(string master, string type, string slave)
        {
            var comm1 = new NpgsqlCommand("insert into relationship values(@master,@type,@slave)", _connection);
            comm1.Parameters.AddWithValue("@master", master);
            comm1.Parameters.AddWithValue("@type", type);
            comm1.Parameters.AddWithValue("@slave", slave);
            comm1.ExecuteNonQuery();
        }
    }
}
