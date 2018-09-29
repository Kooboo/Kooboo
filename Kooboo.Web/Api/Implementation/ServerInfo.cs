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

        public string Api(ApiCall call)
        {
            string value = Lib.Helper.JsonHelper.Serialize(Kooboo.Data.AppSettings.ApiResource);
            return value;
        }
    }
}
