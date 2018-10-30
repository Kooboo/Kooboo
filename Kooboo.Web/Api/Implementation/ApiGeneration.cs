using Kooboo.Api;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class ApiGeneration : IApi
    {
        public string ModelName => "ApiGeneration";

        public bool RequireSite => true;

        public bool RequireUser => true;

        // return Database and TextContent Now. 
        public Dictionary<String, string> Types(ApiCall call)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var db = Kooboo.Data.Language.Hardcoded.GetValue("database", call.Context);
            var txtContent = Kooboo.Data.Language.Hardcoded.GetValue("textcontent", call.Context);    
            result.Add("Database", db);
            result.Add("TextContent", txtContent);
            return result;
        }
                   
        // Return database table names or TextContent Folder name.s 
        public List<string> Objects(string type, ApiCall call)
        {
            string lower = type.ToLower(); 
            if (lower == "database")
            {
                var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);
                var list = db.GetTables();

                list.RemoveAll(o => o.StartsWith("_sys_"));

                list.RemoveAll(o => o.StartsWith("_koobootemp"));

                return list;
            }
            else if (lower == "textcontent")
            {
                var db = call.Context.WebSite.SiteDb();
                var folder = db.ContentFolders.All();

                return folder.Select(o => o.Name).ToList();    
            }

            return null; 
        }
               
        public List<string> Actions(string type, ApiCall call)
        {
            List<string> result = new List<string>();

            result.Add("add");
            result.Add("update");
            result.Add("delete");
            result.Add("get");
            result.Add("query");       
            return result;    
        }

        public bool Generate(string type, string name, List<string> actions, ApiCall call)
        {    
            if (type== "database")
            {
                Kooboo.Web.JQL.CodeGeneration.GenerateDatabase(call.WebSite, name, actions);   
            }
            else if (type== "textcontent")
            {
                Kooboo.Web.JQL.CodeGeneration.GenerateTextContent(call.WebSite, name, actions); 
            }
            return true; 
        }      
        
    }
}
