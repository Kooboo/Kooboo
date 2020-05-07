//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Web.Frontend.KScriptDefine;
using KScript;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class KScriptApi : IApi
    {

        private readonly Lazy<string> _defineContent;

        public KScriptApi()
        {
            _defineContent = new Lazy<string>(() => new KScriptToTsDefineConventer().Convent(typeof(KScript.k)), true);
        }

        public string ModelName
        {
            get { return "KScript"; }
        }

        public bool RequireSite
        {
            get { return false; }
        }

        public bool RequireUser
        {
            get { return false; }
        }

        public string GetDefine()
        {
            return _defineContent.Value;
        }

        public IEnumerable<string> GetTables(ApiCall apiCall, string database)
        {
            var kInstance = new k(apiCall.Context);

            switch (database.ToLower())
            {
                case "database":
                    return new DataBaseApi().Tables(apiCall);
                case "sqlite":
                    return kInstance.Sqlite.Query("SELECT name FROM sqlite_master WHERE type='table';").Select(s => s.GetValue("name").ToString());
                case "mysql":
                    return kInstance.Mysql.Query($"SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_SCHEMA in (select database()) AND TABLE_TYPE = 'BASE TABLE';").Select(s => s.GetValue("TABLE_NAME").ToString());
                case "sqlserver":
                    return kInstance.SqlServer.Query("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';").Select(s => s.GetValue("TABLE_NAME").ToString());
                case "mongo":
                    return kInstance.Mongo.Tables;
                default:
                    return new string[0];
            }
        }

        public IEnumerable<string> GetColumns(ApiCall apiCall, string database, string table)
        {
            var kInstance = new k(apiCall.Context);

            switch (database.ToLower())
            {
                case "database":
                    return new DataBaseApi().Columns(table, apiCall).Select(s => s.Name);
                case "sqlite":
                    return kInstance.Sqlite.Query($"SELECT \"name\" FROM pragma_table_info ('{table}');").Select(s => s.GetValue("name").ToString());
                case "mysql":
                    return kInstance.Mysql.Query($"DESCRIBE `{table}`").Select(s => s.GetValue("Field").ToString());
                case "sqlserver":
                    return kInstance.SqlServer.Query($"SELECT COLUMN_NAME from INFORMATION_SCHEMA.columns where TABLE_NAME= '{table}'").Select(s => s.GetValue("COLUMN_NAME").ToString());
                case "mongo":
                    table= kInstance.Mongo.Tables.FirstOrDefault(f => f == table);
                    if (table == null) return new string[0];
                    return (kInstance.Mongo.GetTable(table) as MongoTable).GetAllField().Select(s=>s.ToString());
                default:
                    return new string[0];
            }
        }
    }
}
