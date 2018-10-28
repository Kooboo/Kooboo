using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.JQL
{
    public static class CodeGeneration
    {
        public static string DatabaseAdd(WebSite Site, string tablename)
        {
            var fields = GetDatabaseFields(Site, tablename);
            if (fields != null && fields.Any())
            {
                return GetDatabaseAdd(tablename, fields); 
            }
            return null; 
        }

        internal static string GetDatabaseAdd(string tablename, List<string> fields)
        {
            string js = "var obj={}; \r\n";
            foreach (var item in fields)
            {
                js += "obj." + item + "=k.request." + item + ";\r\n"; 
            }

            js += "var id = k.database." + tablename + ".add(obj); \r\n";
            js += "k.response.write(id);";    
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

    }
}
