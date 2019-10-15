//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;

namespace Kooboo.Events
{
    public class WebSiteChange : Kooboo.Data.Events.IEvent
    {
        public Kooboo.Data.Models.WebSite WebSite { get; set; }
        public ChangeType ChangeType { get; set; }

        public WebSite OldWebSite { get; set; }
    }
}