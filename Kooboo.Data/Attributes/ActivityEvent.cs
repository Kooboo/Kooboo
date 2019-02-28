//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public   class ActivityEvent : Attribute
    {
        public string GroupName { get; set; }

        public string ActivityName { get; set; }

        public ActivityEvent(string GroupName, string ActivityName)
        {
            this.GroupName = GroupName;
            this.ActivityName = ActivityName; 
        }
    }

    public class ActivityInfo
    {
        public string GroupName { get; set; }

        public string ActivityName { get; set; }
    }


   
}
