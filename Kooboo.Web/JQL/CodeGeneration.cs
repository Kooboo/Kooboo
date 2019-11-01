//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.JQL
{
    public static class CodeGeneration
    {
        public static string DatabaseAddScript(WebSite site, string tablename)
        {
            var fields = GetDatabaseFields(site, tablename);
            if (fields != null && fields.Any())
            {
                return GetDatabaseAdd(tablename, fields);
            }
            return null;
        }

        public static string DatabaseUpdateScript(WebSite site, string tablename)
        {
            var fields = GetDatabaseFields(site, tablename);
            if (fields != null && fields.Any())
            {
                return GetDatabaseUpdate(tablename, fields);
            }
            return null;
        }

        public static string DatabaseDeleteScript(WebSite site, string tablename)
        {
            string js = "var id=k.request.id; \r\n";
            js += "k.database." + tablename + ".delete(id);\r\n";
            return js;
        }

        public static string DatabaseGetScript(WebSite site, string tablename)
        {
            string js = "var id = k.request.id; \r\n";
            js += " var obj = k.database." + tablename + ".get(id);\r\n";
            js += "if (obj) {  k.response.json(obj); }; ";
            return js;
        }

        public static string DatabaseListScript(WebSite site, string tablename)
        {
            string js = " var list = k.database." + tablename + ".all();\r\n";
            js += "if (list) {  k.response.json(list); }; ";
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
                js += "if (k.request." + item + ") {";

                js += "obj." + item + "=k.request." + item + ";}\r\n";
            }

            js += "var id; \r\n";
            js += "if (k.request._id)\r\n";
            js += "{ id = k.request._id; }\r\n";
            js += "else  { id = k.request.id; } \r\n";

            js += "k.database." + tablename + ".update(id, obj); \r\n";
            // js += "k.response.write(id);";
            return js;
        }

        internal static string GetDatabaseDelete(string tablename, string nameOrId)
        {
            string js = "k.database." + tablename + ".delete(" + nameOrId + ");\r\n";
            return js;
        }

        public static List<String> GetDatabaseFields(WebSite site, string tablename)
        {
            var db = Kooboo.Data.DB.GetKDatabase(site);

            var table = Kooboo.Data.DB.GetTable(site, tablename);

            if (table != null)
            {
                return table.Setting.Columns.Where(o => !o.IsSystem).Select(o => o.Name).ToList();
            }
            return null;
        }


        public static void GenerateDatabase(WebSite website, string tableName, List<string> actions)
        {
            foreach (var item in actions)
            {
                GenerateDatabase(website, tableName, item);
            }
        }

        public static void GenerateDatabase(WebSite website, string tableName, string item)
        {
            var sitedb = website.SiteDb();
            string scriptbody = null;
            switch (item)
            {
                case "add":
                    scriptbody = DatabaseAddScript(website, tableName);
                    break;
                case "update":
                    scriptbody = DatabaseUpdateScript(website, tableName);
                    break;
                case "get":
                    scriptbody = DatabaseGetScript(website, tableName);
                    break;
                case "delete":
                    scriptbody = DatabaseDeleteScript(website, tableName);
                    break;
                case "list":
                    scriptbody = DatabaseListScript(website, tableName);
                    break;
            }

            if (scriptbody != null)
            {
                AddDatabaseApi_code(sitedb, tableName, item, scriptbody);
            }
        }

        public static void AddDatabaseApi_code(SiteDb sitedb, string tableName, string actionName, string codebody)
        {
            string url = "/dbapi/" + tableName + "/" + actionName;

            Code code = new Code {Name = "dbapi_" + tableName + "_" + actionName, Body = codebody};

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

            var route = new Kooboo.Sites.Routing.Route
            {
                Name = url, objectId = code.Id, DestinationConstType = ConstObjectType.Code
            };
            sitedb.Routes.AddOrUpdate(route);

        }


        public static void GenerateTextContent(WebSite website, string folderName, List<string> actions)
        {
            foreach (var item in actions)
            {
                GenerateTextContent(website, folderName, item);
            }
        }

        public static void GenerateTextContent(WebSite website, string folderName, string item)
        {
            var sitedb = website.SiteDb();

            string scriptbody = null;
            switch (item)
            {
                case "add":
                    scriptbody = TextContentAddScript(website, folderName);
                    break;
                case "update":
                    scriptbody = TextContentUpdateScript(website, folderName);
                    break;
                case "get":
                    scriptbody = TextContentGetScript(website, folderName);
                    break;
                case "delete":
                    scriptbody = TextContentDeleteScript(website, folderName);
                    break;
                case "list":
                    scriptbody = TextContentListScript(website, folderName);
                    break;
            }

            if (scriptbody != null)
            {
                AddTextContentApi_code(sitedb, folderName, item, scriptbody);
            }
        }



        public static void AddTextContentApi_code(SiteDb sitedb, string folderName, string actionName, string codebody)
        {
            string url = "/textapi/" + folderName + "/" + actionName;

            Code code = new Code {Name = "textapi_" + folderName + "_" + actionName, Body = codebody};

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

            var route = new Kooboo.Sites.Routing.Route
            {
                Name = url, objectId = code.Id, DestinationConstType = ConstObjectType.Code
            };
            sitedb.Routes.AddOrUpdate(route);
        }


        public static string TextContentAddScript(WebSite site, string folderName)
        {
            var fields = GetTextContentFields(site, folderName);
            if (fields != null && fields.Any())
            {
                string js = "var obj={}; \r\n";
                foreach (var item in fields)
                {
                    js += "obj." + item + "=k.request." + item + ";\r\n";
                }
                js += "obj.folder='" + folderName + "'\r\n";
                js += "k.site.textContents.add(obj); \r\n";
                // js += "k.response.write('ok');"; 

                return js;
            }
            return null;
        }

        public static string TextContentUpdateScript(WebSite site, string tablename)
        {
            var fields = GetTextContentFields(site, tablename);
            if (fields != null && fields.Any())
            {

                string js = JsKey();

                js += "if (id) {\r\n";

                js += "var obj=k.site.textContents.get(id); \r\n";

                js += "if (obj) {\r\n";

                foreach (var item in fields)
                {
                    js += "if (k.request." + item + ") {";

                    js += "obj." + item + "=k.request." + item + ";}\r\n";
                }
                js += "k.site.textContents.update(obj); \r\n";

                js += "}\r\n";

                js += "}";

                return js;


            }
            return null;
        }

        private static string JsKey()
        {
            string js = "var id;\r\n";
            js += "if (k.request.id)\r\n";
            js += " { id = k.request.id; }\r\n";
            js += "else if (k.request.usekey)\r\n";
            js += "{ id = k.request.userkey; }\r\n";
            return js;
        }

        public static string TextContentDeleteScript(WebSite site, string tablename)
        {
            string js = JsKey();
            js += "k.site.textContents.delete(id);\r\n";
            return js;
        }

        public static string TextContentGetScript(WebSite site, string tablename)
        {
            string js = JsKey();

            js += "var obj = k.site.textContents.get(id);\r\n";
            js += "if (obj) {  k.response.json(obj); }; ";
            return js;
        }

        public static string TextContentListScript(WebSite site, string folderName)
        {

            string js = " var list = k.site.textContents.findAll(\"folder=='" + folderName + "'\");\r\n";
            js += "if (list) {  k.response.json(list); }; ";
            return js;
        }

        public static List<String> GetTextContentFields(WebSite site, string folderName)
        {
            var db = site.SiteDb();
            var folder = db.ContentFolders.Get(folderName);
            if (folder != null)
            {
                var type = db.ContentTypes.Get(folder.ContentTypeId);

                if (type != null)
                {
                    return type.Properties.Where(o => !o.IsSystemField).Select(o => o.Name).ToList();
                }
            }
            return null;
        }


    }
}
