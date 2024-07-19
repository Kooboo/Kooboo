//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;

namespace Kooboo.Web.Api.Implementation
{
    public class ServerInfoApi : IApi
    {
        public string ModelName => "ServerInfo";

        public bool RequireSite => false;

        public bool RequireUser => false;

        public string Setting(ApiCall call)
        {
            string value = Lib.Helper.JsonHelper.Serialize(Kooboo.Data.AppSettings.ServerSetting);
            return value;
        }

    }
}
