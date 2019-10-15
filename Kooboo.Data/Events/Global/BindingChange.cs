//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Events
{
    public class BindingChange : Kooboo.Data.Events.IEvent
    {
        public Kooboo.Data.Models.Binding Binding { get; set; }
        public ChangeType ChangeType { get; set; }

        public Kooboo.Data.Models.Binding OldBinding { get; set; }
    }
}