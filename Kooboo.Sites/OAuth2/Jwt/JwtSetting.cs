using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authorization.Module.code.Jwt
{
    public class JwtSetting : ISiteSetting
    {
        public string Secret { get; set; }

        public int Exp { get; set; }

        public bool EnableExp { get; set; }

        public string Name => "JwtSetting";
    }
}
