using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Scripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooboo.Sites.OAuth2.WeChat
{
    public class WeChatLogin : BaseOAuth2<WeChatLoginSetting>
    {
        public WeChatLogin(RenderContext context) : base(context)
        {
        }

        [Description(@"
1.Config

site=>system=>settings=>WeChatLoginSetting

appid:xxx

secret:xxx

callbackCodeName:wxcallback

2.Create callbackCode script

development=>code=>create event code

name:wxcallback

code:

k.response.write(k.request.body)

3.Add page

<div>
    <script engine='kscript'>
        var url = k.oAuth2.weChat.getAuthUrl({
            state: 'custom_state'
        })
    </script>
    <a k-href='url'>wechat login</a>
</div>
")]
        public override string GetAuthUrl(IDictionary<string, object> @params)
        {
            var redirectUrl = $"{Context.WebSite.BaseUrl()}_api/oauth2callback/{nameof(WeChatLogin)}";
            var url = $"https://open.weixin.qq.com/connect/qrconnect?appid={Setting.Appid}&redirect_uri={redirectUrl}&response_type=code&scope=snsapi_login&state=";
            if (@params.TryGetValue("state", out var state)) url += state;
            return url;
        }


        [Description(@"
1.Config

site=>system=>settings=>WeChatLoginSetting

appid:xxx

secret:xxx

callbackCodeName:wxcallback

2.Create callbackCode script

development=>code=>create event code

name:wxcallback

code:

k.response.write(k.request.body)

3.Add page

<div>
    <script engine='kscript'>
        var url = k.oAuth2.weChat.getAuthUrl()
    </script>
    <a k-href='url'>wechat login</a>
</div>
")]
        public string GetAuthUrl() => GetAuthUrl(new Dictionary<string, object>());


        [Description(@"
1.Config

site=>system=>settings=>WeChatLoginSetting

appid:xxx

secret:xxx

callbackCodeName:wxcallback

2.Create callbackCode script

development=>code=>create event code

name:wxcallback

code:

k.response.write(k.request.body)

3.Add page

<div>
    <div id='qrcode_container'></div>
    <script src='https://res.wx.qq.com/connect/zh_CN/htmledition/js/wxLogin.js'></script>
    <div id='json_data' style='display:none'>
        <script engine='kscript'>
            k.response.write(k.oAuth2.weChat.getAuthJson({
                                id:'qrcode_container',
                                state:'custom_state'
                            }))
        </script>
    </div>
    <script>
        var json=document.getElementById('json_data').innerText
        new WxLogin(JSON.parse(json));
    </script>
</div>
")]
        public string GetAuthJson(IDictionary<string, object> @params)
        {
            var redirectUrl = $"{Context.WebSite.BaseUrl()}_api/oauth2callback/{nameof(WeChatLogin)}";
            if (!@params.ContainsKey("appid")) @params.Add("appid", Setting.Appid);
            if (!@params.ContainsKey("scope")) @params.Add("scope", "snsapi_login");
            if (!@params.ContainsKey("redirect_uri")) @params.Add("redirect_uri", redirectUrl);
            return JsonHelper.Serialize(@params);
        }

        [KIgnore]
        public override string Callback(IDictionary<string, object> query)
        {
            var codeScript = Context.WebSite.SiteDb().Code.GetByNameOrId(Setting.CallbackCodeName);
            if (codeScript == null) throw new Exception("Not handle callback script code!");
            query.TryGetValue("code", out var code);
            if (string.IsNullOrWhiteSpace(code.ToString())) throw new Exception("code can't be empty");
            var tokenString = _webClient.DownloadString($"https://api.weixin.qq.com/sns/oauth2/access_token?appid={Setting.Appid}&secret={Setting.Secret}&code={code}&grant_type=authorization_code");
            var token = JsonHelper.Deserialize<Dictionary<string, object>>(tokenString);
            var userInfoString = _webClient.DownloadString($"https://api.weixin.qq.com/sns/userinfo?access_token={token["access_token"]}&openid={token["openid"]}");
            var userInfo = JsonHelper.Deserialize<Dictionary<string, object>>(userInfoString);
            query.Add(nameof(token), token);
            query.Add(nameof(userInfo), userInfo);
            Context.Request.Body = JsonHelper.Serialize(query);
            return Manager.ExecuteCode(Context, codeScript.Body, codeScript.Id);
        }
    }
}
