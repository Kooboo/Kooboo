//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Scripting.Global;

namespace KScript
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
            var tb = Kooboo.Data.DB.GetOrCreateTable(db, Name);
            return new KTable(tb, this.context);
        }

        public KTable Table(string Name)
        {
            return GetTable(Name);
        }

        public KTable this[string key]
        {
            get
            {
                return GetTable(key);
            }
            set
            {

            }
        }


    }
}