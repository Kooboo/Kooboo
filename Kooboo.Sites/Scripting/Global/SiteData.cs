//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Sites.Scripting.Global
{
    public class kDataContext
    {
        private RenderContext Context { get; set; }

        public kDataContext(RenderContext context)
        {
            this.Context = context;
        }

        public void set(string key, object value)
        {
            this.Context.DataContext.Push(key, value);
        }

        [Attributes.SummaryIgnore]
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

        public object get(string key)
        {
            return this.Context.DataContext.GetValue(key);
        }
    }
}