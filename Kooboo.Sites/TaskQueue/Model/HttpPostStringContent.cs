//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.TaskQueue
{
 public   class HttpPostStringContent
    { 
        public string RemoteUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        
        public string StringContent { get; set; }
    }
}
