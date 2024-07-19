//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;

namespace Kooboo.Web.Api
{
    public class ReplicaApi : IApi
    {
        public string ModelName
        {
            get { return "Replica"; }
        }

        public bool RequireSite
        {
            get { return false; }
        }
        public bool RequireUser
        {
            get { return false; }
        }

        [Attributes.RequireParameters("Country")]
        public void Create(ApiCall call)
        {
            // check available servers... 
            // we need to know... 
            var site = call.Context.WebSite;

        }

        // when the host add/edit/delete binding.... 
        public void HostUpdateBinding(ApiCall call)
        {

        }

    }
}
