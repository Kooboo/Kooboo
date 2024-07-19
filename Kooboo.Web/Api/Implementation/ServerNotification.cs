//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data;

namespace Kooboo.Web.Api.Implementation
{
    public class ServerNotificationApi : IApi
    {
        public string ModelName => "ServerNotification";
        public bool RequireSite => false;
        public bool RequireUser => false;

        public bool RemoveLocalOrganization(string name, ApiCall call)
        {
            Guid id = Lib.Helper.IDHelper.ParseKey(name);
            GlobalDb.Organization.RemoveOrgCache(id);
            return true;
        }
    }
}
