//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using System.ComponentModel;

namespace KScript
{
    public class kDatabase : IDatabase
    {
        private RenderContext context { get; set; }
        public kDatabase(RenderContext context)
        {
            this.context = context;
        }
       
        public ITable GetTable(string Name)
        {
            var db = Kooboo.Data.DB.GetKDatabase(this.context.WebSite);
            var tb = Kooboo.Data.DB.GetOrCreateTable(db, Name);
            return new KTable(tb, this.context);
        }

        public ITable Table(string Name)
        {
            return GetTable(Name);
        }

        public ITable this[string key]
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