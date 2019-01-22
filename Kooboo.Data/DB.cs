using System.Collections.Generic;
using Kooboo.IndexedDB;
using System;
using Kooboo.Data.Models;


namespace Kooboo.Data
{
    public static class DB
    {

        private static object _dbobject = new object();

        private static Dictionary<string, Database> _databasedictionary = new Dictionary<string, Database>();

        private static Dictionary<string, Database> _tabledb = new Dictionary<string, Database>();

        public static Database GetTableDatabase(string fullpath)
        {
            fullpath = fullpath.ToValidPath();

            if (!_tabledb.ContainsKey(fullpath))
            {
                lock (_dbobject)
                {
                    if (!_tabledb.ContainsKey(fullpath))
                    {
                        Database db = new Database(fullpath);
                     
                        _tabledb.Add(fullpath, db);
                    }
                }
            }

            return _tabledb[fullpath];
        }

        public static bool HasKDatabase(WebSite website)
        {
            string orgfolder = AppSettings.GetOrganizationFolder(website.OrganizationId);
            string sitefolder = System.IO.Path.Combine(orgfolder, website.Name);
            string name = System.IO.Path.Combine(sitefolder, "Tables");

            name = name.ToValidPath();

            return _tabledb.ContainsKey(name); 

        }

        public static Database GetKDatabase(WebSite website)
        {  
            string orgfolder = AppSettings.GetOrganizationFolder(website.OrganizationId);
            string sitefolder = System.IO.Path.Combine(orgfolder, website.Name);
            string name = System.IO.Path.Combine(sitefolder, "Tables"); 
            return Kooboo.Data.DB.GetTableDatabase(name);
        }

        /// <summary>
        /// the name of database,normally it is the site name. 
        /// If database not exists, it will be created. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        private static object _bindingobject = new object();
        private static object _websiteobject = new object();

        /// <summary>
        /// the global databsae for global setting. 
        /// </summary>
        /// <returns></returns>
        public static Database Global()
        {
            return GetDatabase(DataConstants.KoobooGlobalDb);
        }

        public static ObjectStore<Guid, Notification> Notifications()
        {
            string storename = typeof(Notification).Name;

            lock (_websiteobject)
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<Notification>(o => o.Id);
                paras.AddColumn<Notification>(o => o.UserId);
                paras.AddColumn<Notification>(o => o.WebSiteId);
                return Global().GetOrCreateObjectStore<Guid, Notification>(storename, paras);
            }
        }
    }

}
