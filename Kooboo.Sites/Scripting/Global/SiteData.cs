//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Sync;
using System;
using System.ComponentModel;
using System.IO;

namespace KScript
{
    public class kDataContext 
    {
        private RenderContext context { get; set; }
        public kDataContext(RenderContext context)
        {
            this.context = context; 
        } 

        [Description(@"Set value into KView render engine
    var obj = {name: ""myname"", fieldtwo: ""value""};
    k.dataContext.set(""key"", obj);")]
        public void set(string key, object value)
        {
            this.context.DataContext.Push(key, value); 
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public object this[string key]
        {
            get
            {
                return this.get(key); 
            }
            set
            {
                this.set(key, value); 
            }
        }

        [Description("get existing object from dataContext")]
        public object get(string key)
        {
            return this.context.DataContext.GetValue(key); 
        }

 
    }
}
