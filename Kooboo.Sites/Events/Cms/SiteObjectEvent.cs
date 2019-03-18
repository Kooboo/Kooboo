//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Repository;

namespace Kooboo.Events.Cms
{
    public class SiteObjectEvent<T> : SiteObjectEvent
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


    public class SiteObjectEvent : Kooboo.Data.Events.IEvent
    {
        public ChangeType ChangeType { get; set; }

        public ISiteObject Value { get; set; }
        /// <summary>
        /// The old value when it is an update event.
        /// </summary>
        public ISiteObject OldValue { get; set; }
         
        public SiteDb SiteDb { get; set; }

        public long Position { get; set; }
        public long OldPosition { get; set; }
    }
}
