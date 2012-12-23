// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Data.Linq;
using AutoX.Basic.Model;

#endregion

namespace AutoX.Database
{
    public class DB : DataContext
    {
        public Table<Datum> Data;
        public Table<Folder> Folders;
        public Table<Index> Indexes;

        public Table<Result> Results;
        public Table<Script> Scripts;
        public Table<Translation> Translations;
        public Table<UIObject> UIPool;

        public DB(string connection) : base(connection)
        {
        }
    }
}