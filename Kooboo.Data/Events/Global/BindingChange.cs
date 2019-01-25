//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Events
{
    public class BindingChange : Kooboo.Data.Events.IEvent
    {
        public Kooboo.Data.Models.Binding binding { get; set; }
        public ChangeType ChangeType { get; set; }

        public Kooboo.Data.Models.Binding OldBinding { get; set; }
    }
}
