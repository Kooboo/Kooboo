using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Scripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooboo.Sites.OAuth2.Weibo
{
    public class WeiboLogin : BaseOAuth2<WeiboSetting>
    {
        public WeiboLogin(RenderContext context) : base(context)
        {
        }

        [Description(@"
1.Config
site=>system=>settings=>WeibotLoginSetting

appid:xxx
secret:xxx
callbackCodeName:wbcallback

2.Create callbackCode script
development=>code=>create event code

name:wbcallback
code:
k.response.write(k.request.body)

3.Add page
<div>
    <script engine='kscript'>
        var url = k.oAuth2.weibo.getAuthUrl()
    </script>
    <a k-href='url'>weibo login</a>
</div>
")]
        public string GetAuthUrl() => GetAuthUrl(new Dictionary<string, object>());

        [KIgnore]
        public override string GetAuthUrl(IDictionary<string, object> @params)
        {
            var redirectUrl = $"{Context.WebSite.BaseUrl()}_api/oauth2callback/{nameof(WeiboLogin)}";
            var url = $"https://api.weibo.com/oauth2/authorize?client_id={Setting.Appid}&redirect_uri={redirectUrl}&response_type=code";
            return url;
        }

        [KIgnore]
        public override string Callback(IDictionary<string, object> query)
        {
            var codeScript = Context.WebSite.SiteDb().Code.GetByNameOrId(Setting.CallbackCodeName);
            if (codeScript == null) throw new Exception("Not handle callback script code!");
            query.TryGetValue("code", out var code);
            if (string.IsNullOrWhiteSpace(code.ToString())) throw new Exception("code can't be empty");
            var redirectUrl = $"{Context.WebSite.BaseUrl()}_api/oauth2callback/{nameof(WeiboLogin)}";
            var tokenString = _webClient.UploadString($"https://api.weibo.com/oauth2/access_token?client_id={Setting.Appid}&client_secret={Setting.Secret}&grant_type=authorization_code&redirect_uri={redirectUrl}&code={code}", "");
            var token = JsonHelper.Deserialize<Dictionary<string, object>>(tokenString.ToString());
            var userInfoString = _webClient.DownloadString($"https://api.weibo.com/2/users/show.json?access_token={token["access_token"]}&uid={token["uid"]}");
            var userInfo = JsonHelper.Deserialize<Dictionary<string, object>>(userInfoString);
            query.Add(nameof(token), token);
            query.Add(nameof(userInfo), userInfo);
            Context.Request.Body = JsonHelper.Serialize(query);
            return Manager.ExecuteCode(Context, codeScript.Body, codeScript.Id);
        }
    }
}
