using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.SMS
{
    public class TencentSMSSetting : ISiteSetting
    {
        public string Name => "TencentSMSSetting";

        //var setting = RenderContext.WebSite.SiteDb().CoreSetting.GetSetting<MysqlSetting>();

         public string secretId { get; set; }

        public string secretKey { get; set; }

        public string appId { get; set; }

        public string sign { get; set; }

        public string regionId { get; set; } 

    }
}
