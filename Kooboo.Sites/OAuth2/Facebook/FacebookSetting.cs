using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.OAuth2.Facebook
{
    public class FacebookSetting : ISiteSetting
    {
        public string Appid { get; set; }
        public string Secret { get; set; }
        public string CallbackCodeName { get; set; }
        public string Fields { get; set; }

        public string Name => "FacebookLoginSetting";
    }
}
