//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.IndexedDB;
using System;
using Kooboo.Data.Models;
using Kooboo.IndexedDB.Dynamic;

namespace Kooboo.Data
{
    public static class DB
    {

        private static object _dbobject = new object();

        private static Dictionary<string, Database> _databasedictionary { get; set; } = new Dictionary<string, Database>();


        public static Database GetKDatabase(WebSite website)
        {
            return GetDatabase(website);
        }

        public static Database GetDatabase(string name)
        {
            name = name.ToValidPath();

            if (!_databasedictionary.ContainsKey(name))
            {
                lock (_dbobject)
                {
                    if (!_databasedictionary.ContainsKey(name))
                    {
                        Database db = new Database(name);
                        _databasedictionary.Add(name, db);
                    }
                }
            }

            return _databasedictionary[name];
        }

        public static Database GetDatabase(WebSite site)
        {
            var dbName = AppSettings.GetDbName(site.OrganizationId, site.Name);
            return GetDatabase(dbName);
        }

        public static void DeleteDatabase(string name)
        {
            name = name.ToValidPath();

            if (_databasedictionary.ContainsKey(name))
            {
                var db = _databasedictionary[name];
                db.deleteDatabase();
                _databasedictionary.Remove(name);
            }
            else
            {
                Database db = new Database(name);
                if (db.Exists)
                {
                    db.deleteDatabase();
                    db = null;
                }
            }

        }

        /// <summary>
        /// the global databsae for global setting. 
        /// </summary>
        /// <returns></returns>
        public static Database Global()
        {
            return GetDatabase(DataConstants.KoobooGlobalDb);
        }

        public static Table GetOrCreateTable(Database database, string tablename, IndexedDB.Dynamic.Setting setting = null)
        {
            if (setting == null)
            {
                setting = GetDefaultTableSetting(tablename);
            }

            return database.GetOrCreateTable(tablename, setting);
        }

        public static Table GetOrCreateTable(WebSite site, string tablename, IndexedDB.Dynamic.Setting setting = null)
        {
            var kdb = GetKDatabase(site);

            return GetOrCreateTable(kdb, tablename, setting);
        }

        private static IndexedDB.Dynamic.Setting GetDefaultTableSetting(string tablename)
        {
            string lower = tablename.ToLower();
            if (lower == "_sys_keyvalues")
            {
                IndexedDB.Dynamic.Setting setting = new IndexedDB.Dynamic.Setting();
                setting.AppendColumn("key", typeof(string), 256);
                setting.AppendColumn("value", typeof(string), int.MaxValue);
                //setting.SetPrimaryKey("key", typeof(string), 256);
                return setting;
            }
            return null;
        }

        public static Table GetTable(Database database, string tableName)
        {  
            if (!database.HasTable(tableName))
            {
                return null;
            }
            else
            {
                return database.GetOrCreateTable(tableName);
            } 
        }


        public static Table GetTable(WebSite site, string tableName)
        {
            var database = GetKDatabase(site);

            return GetTable(database, tableName); 
        }

    }

}
