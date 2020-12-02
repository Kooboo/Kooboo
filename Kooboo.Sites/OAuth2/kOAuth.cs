using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.OAuth2.Google;
using Kooboo.Sites.OAuth2.WeChat;
using Kooboo.Sites.OAuth2.Weibo;
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

        private Lazy<WeiboLogin> _weiboLogin;

        public WeiboLogin Weibo => _weiboLogin.Value;

        private Lazy<GoogleLogin> _googleLogin;

        public GoogleLogin Google => _googleLogin.Value;

        private Lazy<FacebookLogin> _facebookLogin;

        public FacebookLogin Facebook => _facebookLogin.Value;

        public kOAuth()
        {
            _weChatLogin = new Lazy<WeChatLogin>(() => new WeChatLogin(context), true);
            _weiboLogin = new Lazy<WeiboLogin>(() => new WeiboLogin(context), true);
            _googleLogin = new Lazy<GoogleLogin>(() => new GoogleLogin(context), true);
            _facebookLogin = new Lazy<FacebookLogin>(() => new FacebookLogin(context), true);
        }
    }
}
