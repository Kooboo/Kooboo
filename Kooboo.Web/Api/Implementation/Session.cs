//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation
{
    public class SessionApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "session"; 
            }
        }

        public bool RequireSite
        {
            get
            {
                return false; 
            }
        }

        public bool RequireUser
        {
            get
            {
                return false; 
            }
        }

        public virtual Kooboo.Data.Session.Requirement Requirement(ApiCall call)
        {
            // return the session requirement. 
           return new Data.Session.Requirement() { };    
        }
          

    }
}
