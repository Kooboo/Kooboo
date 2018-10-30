using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.JQL
{
    public static class CodeGeneration
    {
        public static string DatabaseAddScript(WebSite Site, string tablename)
        {
            var fields = GetDatabaseFields(Site, tablename);
            if (fields != null && fields.Any())
            {
                return GetDatabaseAdd(tablename, fields);
            }
            return null;
        }

        public static string DatabaseUpdateScript(WebSite Site, string tablename)
        {
            var fields = GetDatabaseFields(Site, tablename);
            if (fields != null && fields.Any())
            {
                return GetDatabaseUpdate(tablename, fields);
            }
            return null;
        }

        public static string DatabaseDeleteScript(WebSite Site, string tablename)
        {
            string js = "var id; \r\n";
            js += "if (k.request._id)\r\n";
            js += "{ id = k.request._id; }\r\n";
            js += "else  { id = k.request.id; } \r\n";
            js += "k.database." + tablename + ".delete(id);\r\n";
            return js;
        }

        public static string DatabaseGetScript(WebSite Site, string tablename)
        {
            string js = "var id; \r\n";
            js += "if (k.request._id)\r\n";
            js += "{ id = k.request._id; }\r\n";
            js += "else  { id = k.request.id; } \r\n";
            js += " var obj = k.database." + tablename + ".get(id);\r\n";
            js += "if (obj) {  k.response.json(obj); }; ";
            return js;
        }
                                  
        public static string DatabaseListScript(WebSite Site, string tablename)
        {
            string js = " var list = k.database." + tablename + ".all();\r\n";
            js += "if (list) {  k.json(obj); }; ";
            return js;
        }

        internal static string GetDatabaseAdd(string tablename, List<string> fields)
        {
            string js = "var obj={}; \r\n";
            foreach (var item in fields)
            {
                js += "obj." + item + "=k.request." + item + ";\r\n";
            }

            js += "var id = k.database." + tablename + ".add(obj); \r\n";
            // js += "k.response.write(id);";
            return js;
        }

        internal static string GetDatabaseUpdate(string tablename, List<string> fields)
        {
            string js = "var obj={}; \r\n";
            foreach (var item in fields)
            {
                js += "obj." + item + "=k.request." + item + ";\r\n";
            }

            js += "var id; \r\n";
            js += "if (k.request._id)\r\n";
            js += "{ id = k.request._id; }\r\n";
            js += "else  { id = k.request.id; } \r\n";

            js += "k.database." + tablename + ".update(id, obj); \r\n";
            // js += "k.response.write(id);";
            return js;
        }

        internal static string GetDatabaseDelete(string tablename, string NameOrId)
        {
            string js = "k.database." + tablename + ".delete(" + NameOrId + ");\r\n";
            return js;
        }

        public static List<String> GetDatabaseFields(WebSite site, string tablename)
        {
            var db = Kooboo.Data.DB.GetKDatabase(site);
            var list = db.GetTables();
            if (list.Contains(tablename))
            {
                var table = db.GetOrCreateTable(tablename);
                return table.Setting.Columns.Where(o => !o.IsSystem).Select(o => o.Name).ToList();
            }
            return null;
        }

        public static void GenerateTextContent(WebSite website, string TableName, List<string> actions)
        {
            return; 
        }

        public static void GenerateDatabase(WebSite website, string TableName, List<string> actions)
        {        
            foreach (var item in actions)
            {
                GenerateDatabase(website, TableName, item);
            }
        }

        private static void GenerateDatabase(WebSite website, string TableName, string item)
        {
            var sitedb = website.SiteDb();
            string scriptbody = null;
            if (item == "add")
            {
                scriptbody = DatabaseAddScript(website, TableName);
            }
            else if (item == "update")
            {
                scriptbody = DatabaseUpdateScript(website, TableName);
            }
            else if (item == "get")
            {
                scriptbody = DatabaseGetScript(website, TableName);
            }
            else if (item == "delete")
            {
                scriptbody = DatabaseDeleteScript(website, TableName);
            }
            else if (item == "list")
            {
                scriptbody = DatabaseListScript(website, TableName);
            }

            if (scriptbody != null)
            {
                AddDatabaseApi_code(sitedb, TableName, item, scriptbody);
            }
        }

        public static void AddDatabaseApi_code(SiteDb sitedb, string tableName, string actionName, string codebody)
        {
            string url = "/kb_api/" + tableName + "_" + actionName;

            Code code = new Code();
            code.Name = "kb_" + tableName + "_" + actionName;
            code.Body = codebody;

            var oldcode = sitedb.Code.Get(code.Id);
            if (oldcode != null)
            {
                return; // already exists. 
            }

            code.CodeType = CodeType.Api;

            if (code.CodeType == Sites.Models.CodeType.Api)
            {
                if (!sitedb.Routes.Validate(url, code.Id))
                {
                    // already exists... 
                    return;
                }
            }


            sitedb.Code.AddOrUpdate(code);


            var route = new Kooboo.Sites.Routing.Route();
            route.Name = url;
            route.objectId = code.Id;
            route.DestinationConstType = ConstObjectType.Code;
            sitedb.Routes.AddOrUpdate(route);

        }




    }
}
