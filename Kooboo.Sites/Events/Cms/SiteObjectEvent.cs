using Kooboo.Sites.Repository;
using Kooboo.Data.Interface;

namespace Kooboo.Events.Cms
{
    public class SiteObjectEvent : Kooboo.Data.Events.IEvent
    {
        public ChangeType ChangeType { get; set; }
        
        public  ISiteObject Value { get; set; }
        /// <summary>
        /// The old value when it is an update event.
        /// </summary>
        public ISiteObject OldValue { get; set; }

    
        public SiteDb SiteDb { get; set; } 
    } 
}
