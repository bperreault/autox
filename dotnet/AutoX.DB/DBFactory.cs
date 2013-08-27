#region

using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Basic.FeatureToggles;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

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
        public static MemcachedFeature memcachedFeature = new MemcachedFeature();
        private static readonly IData Data = null;
        private static MemcachedClient memcachedClient;
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

        public static string GetMemcachedData(string id)
        {
            return !memcachedFeature.FeatureEnabled ? null : GetMemcachedClient().Get<string>(id);
        }

        public static bool DeleteMemcachedData(string id)
        {
            return memcachedFeature.FeatureEnabled && GetMemcachedClient().Remove(id);
        }

        public static bool UpdateMemcachedData(string id, string data)
        {
            if (!memcachedFeature.FeatureEnabled) return false;
            if (GetMemcachedClient().Get(id) != null)
                return GetMemcachedClient().Store(StoreMode.Replace, id, data);
            return GetMemcachedClient().Store(StoreMode.Add, id, data);
        }

        public static List<string> GetMemcachedRelationship(string parentId)
        {
            if (!memcachedFeature.FeatureEnabled)
                return null;
            if (GetMemcachedClient().Get<List<string>>("Parent_" + parentId) == null)
                return null;
            return GetMemcachedClient().Get<List<string>>("Parent_" + parentId);
        }

        public static bool DeleteMemcachedRelationship(string parentId, string childId)
        {
            if (!memcachedFeature.FeatureEnabled)
                return true;
            if (GetMemcachedClient().Get<List<string>>("Parent_" + parentId) == null)
                return true;
            var kids = GetMemcachedClient().Get<List<string>>("Parent_" + parentId);
            kids.Remove(childId);
            return true;
        }

        public static bool DeleteMemcachedRelationship(string parentId)
        {
            if (!memcachedFeature.FeatureEnabled)
                return true;
            return GetMemcachedClient().Remove("Parent_" + parentId);
        }

        public static bool AddMemcachedRelationship(string parentId, string childId)
        {
            if (!memcachedFeature.FeatureEnabled)
                return false;
            if (GetMemcachedClient().Get<List<string>>("Parent_" + parentId) == null)
                GetMemcachedClient().Store(StoreMode.Add, "Parent_" + parentId, new List<string>());
            var kids = GetMemcachedClient().Get<List<string>>("Parent_" + parentId);
            kids.Add(childId);
            return true;
        }

        public static MemcachedClient GetMemcachedClient()
        {
            if (!memcachedFeature.FeatureEnabled)
                return null;
            if (memcachedClient != null) return memcachedClient;
            var config = new MemcachedClientConfiguration();
            var servers = Configuration.Settings("MemcachedServers", "localhost");
            var server = servers.Split(';');
            foreach (var s in server)
            {
                config.Servers.Add(GetIPEndPointFromHostName(s,11211,true));
            }
            config.Protocol = MemcachedProtocol.Text;
            memcachedClient = new MemcachedClient(config);
            return memcachedClient;
        }

        public static IPEndPoint GetIPEndPointFromHostName(string hostName, int port, bool throwIfMoreThanOneIP)
        {
            var addresses = Dns.GetHostAddresses(hostName);
            if (addresses.Length == 0)
            {
                Log.Error(
                    "Unable to retrieve address from specified host name."+hostName);
            }
            else if (throwIfMoreThanOneIP && addresses.Length > 1)
            {
                Log.Error("There is more that one IP address to the specified host."+hostName);
            }
            return new IPEndPoint(addresses[0], port); // Port gets validated here.
        }
    }

   
}