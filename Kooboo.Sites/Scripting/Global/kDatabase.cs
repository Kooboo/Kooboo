//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Sites.Scripting.Global
{
    public class kDatabase
    {
        private RenderContext Context { get; set; }

        public kDatabase(RenderContext context)
        {
            this.Context = context;
        }

        public KTable GetTable(string name)
        {
            var db = Kooboo.Data.DB.GetKDatabase(this.Context.WebSite);
            var tb = Data.DB.GetOrCreateTable(db, name);
            return new KTable(tb, this.Context);
        }

        public KTable Table(string name)
        {
            return GetTable(name);
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