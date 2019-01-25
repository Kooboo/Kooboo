//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
   public class ApiScriptItem
    {
        public string Route { get; set; }

        public string Body { get; set; } 

        public string PreviewUrl { get; set; }

        public Guid Id { get; set; }
    }
}
