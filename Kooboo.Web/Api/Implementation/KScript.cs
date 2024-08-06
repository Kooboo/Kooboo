//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Sites;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Render.Queries;
using Kooboo.Sites.Scripting.Global.Database;
using Kooboo.Sites.Scripting.Helper;
using Kooboo.Sites.Scripting.KDefine;
using Kooboo.Sites.Scripting.KDefine.Html;
using Kooboo.Sites.ScriptModules;
using KScript;
using Microsoft.Extensions.DependencyInjection;

namespace Kooboo.Web.Api.Implementation
{
    public class KScriptApi : IApi
    {
        public string ModelName => "KScript";
        public bool RequireSite => false;
        public bool RequireUser => false;

        static readonly Lazy<TypeDefineConverter> _kDefine = new(() => new TypeDefineConverter(typeof(k)), true);
        static readonly Lazy<string> _kDefineString = new(() => new DefineStringify(_kDefine.Value.Convert()).ToString(), true);

        public string GetDefine(ApiCall apiCall)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_kDefineString.Value);
            sb.AppendLine(Sites.Market.Document.DefineString);

            if (apiCall.WebSite != null)
            {
                sb.AppendLine(Sites.OpenApi.Cache.Get(apiCall.WebSite).DefineString);
                sb.AppendLine(Sites.Scripting.Global.Content.Cache.Get(apiCall.WebSite));
                sb.AppendLine(UserOptionsService.Get(apiCall.Context).DefineString);
                var customDataDefineConverter = new Sites.Commerce.CustomData.DefineConverter(apiCall.Context);
                sb.AppendLine(new DefineStringify(customDataDefineConverter.Convert()).ToString());
            }

            sb.AppendLine($"declare const {AppSettings.KPrefix ?? "k"}: {_kDefine.Value.TypeName};");
            sb.AppendLine(Libs.Vue);
            return sb.ToString();
        }

        public string GetLibs()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Libs.Vue);
            return sb.ToString();
        }

        public IEnumerable<HtmlDefine> GetKViewSuggestions(ApiCall apiCall)
        {
            var define = apiCall.Context.HttpContext.RequestServices.GetService<IDefine<HtmlDefine>>();
            IEnumerable<HtmlDefine> htmlDefines = define.GetListWithCache();
            htmlDefines = htmlDefines.Where(w =>
            {
                if (w.Name == "kview-v") return true;
                if (w.Name == "kview-k") return true;
                if (apiCall.WebSite == default) return true;
                return apiCall.WebSite.CodeSuggestions.Contains(w.Name);
            }).Append(QueryManager.HtmlDefine).ToArray();
            var kView = htmlDefines.FirstOrDefault(f => f.Name == "kview-v");
            var siteDb = apiCall.WebSite?.SiteDb();
            if (kView != default && siteDb != default)
            {
                var prUrl = Data.Service.WebSiteService.EnsureHttpsBaseUrlOnServer(apiCall.WebSite.BaseUrl(), apiCall.WebSite);

                try
                {
                    kView.DynamicTagsSuggest(apiCall.WebSite.SiteDb(), prUrl);
                }
                catch (System.Exception)
                {
                }
            }

            return htmlDefines;
        }

        public JsonTextResponse GetClassSuggestions(ApiCall apiCall)
        {
            var define = apiCall.Context.HttpContext.RequestServices.GetService<IDefine<ClassDefine>>();
            IEnumerable<ClassDefine> classDefine = define.GetListWithCache();
            classDefine = classDefine.Where(w => apiCall.WebSite?.CodeSuggestions.Contains(w.Name) ?? true).ToArray();
            var json = JsonSerializer.Serialize(classDefine, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                Converters ={
                         new JsonStringEnumConverter()
                    }
            });

            return new JsonTextResponse(json);
        }

        public IEnumerable<string> GetTables(ApiCall apiCall, string database)
        {
            database = DynamicDatabaseHelper.GetHintType(database?.ToLower(), apiCall.WebSite);
            var kInstance = new k(apiCall.Context);
            var moduleId = apiCall.GetGuidValue("moduleId");

            return database switch
            {
                "localindexeddb" => GetModuleDatabaseTables(apiCall.Context, moduleId),
                "localsqlite" => GetModuleSqliteTables(apiCall.Context, moduleId),
                "indexeddb" => new DataBaseApi().Tables(apiCall),
                "sqlite" => kInstance.Sqlite.GetTables(),
                "mysql" => kInstance.Mysql.GetTables(),
                "sqlserver" => kInstance.SqlServer.GetTables(),
                "mongo" => kInstance.Mongo.Tables,
                "content" => apiCall.Context.WebSite.SiteDb().ContentFolders.All().Select(s => s.Name),
                _ => Array.Empty<string>(),
            };
        }

        public IEnumerable<string> GetColumns(ApiCall apiCall, string database, string table)
        {
            var kInstance = new k(apiCall.Context);
            var moduleId = apiCall.GetGuidValue("moduleId");

            switch (database.ToLower())
            {
                case "localdatabase":
                    return GetModuleDatabaseColumns(apiCall.Context, table, moduleId);
                case "localsqlite":
                    return GetModuleSqliteColumns(apiCall.Context, table, moduleId);
                case "database":
                    return new DataBaseApi().Columns(table, apiCall).Select(s => s.Name);
                case "sqlite":
                    return kInstance.Sqlite.Query($"SELECT \"name\" FROM pragma_table_info ('{table}');").Select(s => s.GetValue("name").ToString());
                case "mysql":
                    return kInstance.Mysql.Query($"DESCRIBE `{table}`").Select(s => s.GetValue("Field").ToString());
                case "sqlserver":
                    return kInstance.SqlServer.Query($"SELECT COLUMN_NAME from INFORMATION_SCHEMA.columns where TABLE_NAME= '{table}'").Select(s => s.GetValue("COLUMN_NAME").ToString());
                case "mongo":
                    table = kInstance.Mongo.Tables.FirstOrDefault(f => f == table);
                    if (table == null) return new string[0];
                    return (kInstance.Mongo.GetTable(table) as MongoTable).GetAllField().Select(s => s.ToString());
                default:
                    return Array.Empty<string>();
            }
        }

        private IEnumerable<string> GetModuleSqliteColumns(RenderContext context, string tableName, Guid moduleId)
        {
            var module = context.WebSite.SiteDb().ScriptModule.Get(moduleId);
            if (module == null) return Array.Empty<string>();
            var moduleContext = ModuleContext.FromRenderContext(context, module);
            var database = new KModule(context).LocalSqlite;
            return database.Query($"SELECT \"name\" FROM pragma_table_info ('{tableName}');").Select(s => s.GetValue("name").ToString());
        }

        private IEnumerable<string> GetModuleDatabaseColumns(RenderContext context, string tableName, Guid moduleId)
        {
            var module = context.WebSite.SiteDb().ScriptModule.Get(moduleId);
            if (module == null) return Array.Empty<string>();
            var moduleContext = ModuleContext.FromRenderContext(context, module);
            var database = new KModuleDatabase(moduleContext);
            return database.GetColumns(tableName);
        }

        public string[] GetModuleDatabaseTables(RenderContext context, Guid moduleId)
        {
            var module = context.WebSite.SiteDb().ScriptModule.Get(moduleId);
            if (module == null) return Array.Empty<string>();
            var moduleContext = ModuleContext.FromRenderContext(context, module);
            var database = new KModuleDatabase(moduleContext);
            return database.GetTables() ?? Array.Empty<string>();
        }

        public string[] GetModuleSqliteTables(RenderContext context, Guid moduleId)
        {
            var module = context.WebSite.SiteDb().ScriptModule.Get(moduleId);
            if (module == null) return Array.Empty<string>();
            var moduleContext = ModuleContext.FromRenderContext(context, module);
            var database = new KModule(context).LocalSqlite;
            return database.Query("SELECT name FROM sqlite_master WHERE type='table';")
                                            .Select(s => s.GetValue("name").ToString())
                                            .ToArray();
        }
    }
}
