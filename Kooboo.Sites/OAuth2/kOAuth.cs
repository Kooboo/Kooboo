using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.OAuth2.WeChat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.OAuth2
{
    public class kOAuth : IkScript
    {
        [KIgnore]
        public string Name => "OAuth2";

        [KIgnore]
        public RenderContext context { get; set; }

        private Lazy<WeChatLogin> _weChatLogin;

        public WeChatLogin WeChat => _weChatLogin.Value;

        public kOAuth()
        {
            _weChatLogin = new Lazy<WeChatLogin>(() => new WeChatLogin(context), true);
        }
    }
}
