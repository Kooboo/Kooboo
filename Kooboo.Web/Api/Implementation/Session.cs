//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;

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

        public bool ValidDevPassword(ApiCall call)
        {

            var pass = call.Context.Request.Body;

            return Kooboo.Sites.Service.WebSiteService.DevelopmentAccess.TryGrantAccess(call.Context, pass);

            // return AccessControl.ValidDevAccess(call.Context);
        }
    }
}
