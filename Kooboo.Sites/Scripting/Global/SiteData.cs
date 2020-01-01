//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;

namespace KScript
{
    public class kDataContext 
    {
        private RenderContext context { get; set; }
        public kDataContext(RenderContext context)
        {
            this.context = context; 
        } 

        public void set(string key, object value)
        {
            this.context.DataContext.Push(key, value); 
        }

        [Kooboo.Attributes.SummaryIgnore]
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
            return this.context.DataContext.GetValue(key); 
        }
    }
}
