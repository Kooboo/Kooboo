using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System.Linq; 

namespace Kooboo.Sites.Render.Functions
{
    public class Tablefunction : IFunction
    {
        public string Name => "table";

        public List<IFunction> Parameters { get; set; }

        public object Render(RenderContext context)
        {
            if (Parameters == null || !Parameters.Any())
            {
                return null; 
            }

            var namepara = this.Parameters.First();

            var name = namepara.Render(context); 

            if (name == null)
            {
                return null; 
            }
             

            var sitedb = context.WebSite.SiteDb();
            var table = sitedb.DatabaseDb.GetTable(name.ToString()); 

            if (table !=null)
            {
                return table.All(); 
            }

            return null; 

        }
    }
}
