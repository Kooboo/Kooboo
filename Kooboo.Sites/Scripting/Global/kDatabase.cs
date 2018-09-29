using Kooboo.Data.Context;
 
namespace Kooboo.Sites.Scripting.Global
{ 
    public class kDatabase
    {
        private RenderContext context { get; set; }
        public kDatabase(RenderContext context)
        {
            this.context = context;
        }
         
        public KTable GetTable(string Name)
        {
            var db = Kooboo.Data.DB.GetKDatabase(this.context.WebSite);
            var tb =  db.GetOrCreateTable(Name);
            return new KTable(tb); 
        }

        public KTable Table(string Name)
        {
            var db = Kooboo.Data.DB.GetKDatabase(this.context.WebSite);
            var tb = db.GetOrCreateTable(Name);
            return new KTable(tb);
        }
    }  
} 