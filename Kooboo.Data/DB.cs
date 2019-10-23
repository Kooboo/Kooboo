//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Dynamic;
using System.Collections.Generic;

namespace Kooboo.Data
{
    public static class DB
    {
        private static object _dbobject = new object();

        private static Dictionary<string, Database> Databasedictionary { get; set; } = new Dictionary<string, Database>();

        public static Database GetKDatabase(WebSite website)
        {
            return GetDatabase(website);
        }

        public static Database GetDatabase(string name)
        {
            name = name.ToValidPath();

            if (!Databasedictionary.ContainsKey(name))
            {
                lock (_dbobject)
                {
                    if (!Databasedictionary.ContainsKey(name))
                    {
                        Database db = new Database(name);
                        Databasedictionary.Add(name, db);
                    }
                }
            }

            return Databasedictionary[name];
        }

        public static Database GetDatabase(WebSite site)
        {
            var dbName = AppSettings.GetDbName(site.OrganizationId, site.Name);
            return GetDatabase(dbName);
        }

        public static void DeleteDatabase(string name)
        {
            name = name.ToValidPath();

            if (Databasedictionary.ContainsKey(name))
            {
                var db = Databasedictionary[name];
                db.deleteDatabase();
                Databasedictionary.Remove(name);
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
            return !database.HasTable(tableName) ? null : database.GetOrCreateTable(tableName);
        }

        public static Table GetTable(WebSite site, string tableName)
        {
            var database = GetKDatabase(site);

            return GetTable(database, tableName);
        }
    }
}