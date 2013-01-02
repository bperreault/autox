#region

using System;
using AutoX.Basic.Model;
using Newtonsoft.Json;

#endregion

namespace AutoX.DB
{
    public static class Data
    {
        public static IDataObject GetDataObject(Type type, string id)
        {
            var bsonStr = DBManager.GetInstance().find(type, id);
            if (bsonStr == null)
                return null;
            return (IDataObject) JsonConvert.DeserializeObject(bsonStr, type);
        }
    }
}