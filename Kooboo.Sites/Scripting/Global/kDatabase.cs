//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Scripting.Global;
using System.ComponentModel;

namespace KScript
{
    public class kDatabase
    {
        private RenderContext context { get; set; }
        public kDatabase(RenderContext context)
        {
            this.context = context;
        }

        [Description("Return the kScript database table object, if the table is not exists, it will be created.	")]
        public KTable GetTable(string Name)
        {
            var db = Kooboo.Data.DB.GetKDatabase(this.context.WebSite);
            var tb = Kooboo.Data.DB.GetOrCreateTable(db, Name);
            return new KTable(tb, this.context);
        }

        [KIgnore]
        public KTable Table(string Name)
        {
            return GetTable(Name);
        }

        [KIgnore]
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