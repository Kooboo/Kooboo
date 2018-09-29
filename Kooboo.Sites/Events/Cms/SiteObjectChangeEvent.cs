//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;

namespace Kooboo.Events.Cms
{
    public class SiteObjectChangeEvent<T> : SiteObjectEvent
        where T : class, ISiteObject
    {
        public new T OldValue
        {
            get
            {
                return base.OldValue as T;
            }
            set
            {
                base.OldValue = value;
            }
        }

        public new T Value
        {
            get
            {
                return base.Value as T;
            }
            set
            {
                base.Value = value;
            }
        }
    }
}
