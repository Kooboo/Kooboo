using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Jwt
{
    public class JwtSetting : ISiteSetting,ISettingDescription
    {
        public string Secret { get; set; }

        public int Exp { get; set; }

        public bool EnableExp { get; set; }

        public string Name => "JwtSetting";

        public string Group => "Others";

        public string GetAlert(RenderContext renderContext)
        {
            return $@"Example:
Secret: 3800EEB70D3B4110A6B4FBC40D026C9D
Exp: 3600
EnableExp: true
";
        }
    }
}
