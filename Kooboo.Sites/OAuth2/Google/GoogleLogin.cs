using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Scripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace Kooboo.Sites.OAuth2.Google
{
    public class GoogleLogin : BaseOAuth2<GoogleSetting>
    {
        public GoogleLogin(RenderContext context) : base(context)
        {
        }

        [Description(@"
1.Config

site=>system=>settings=>GoogleLoginSetting

appid:xxx

secret:xxx

callbackCodeName:googlecallback

2.Create callbackCode script

development=>code=>create event code

name:googlecallback

code:

k.response.write(k.request.body)

3.Add page

<div>
    <script engine='kscript'>
        var url = k.oAuth2.google.getAuthUrl({
            state:'custom_state'
        })
    </script>
    <a k-href='url'>google login</a>
</div>
")]
        public override string GetAuthUrl(IDictionary<string, object> @params)
        {
            var redirectUrl = $"{Context.WebSite.BaseUrl()}_api/oauth2callback/{nameof(GoogleLogin)}";
            var url = $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&include_granted_scopes=true&client_id={Setting.Appid}&redirect_uri={redirectUrl}";
            @params.TryGetValue("scope", out var scope);
            scope = scope ?? "https://www.googleapis.com/auth/userinfo.profile";
            scope = HttpUtility.UrlEncode(scope.ToString());
            url += $"&scope={scope}";
            if (@params.TryGetValue("state", out var state)) url += $"&state={state}";
            return url;
        }

        [Description(@"
1.Config

site=>system=>settings=>GoogleLoginSetting

appid:xxx

secret:xxx

callbackCodeName:googlecallback

2.Create callbackCode script

development=>code=>create event code

name:googlecallback

code:

k.response.write(k.request.body)

3.Add page

<div>
    <script engine='kscript'>
        var url = k.oAuth2.google.getAuthUrl()
    </script>
    <a k-href='url'>google login</a>
</div>
")]
        public string GetAuthUrl() => GetAuthUrl(new Dictionary<string, object>());

        [KIgnore]
        public override string Callback(IDictionary<string, object> query)
        {
            var codeScript = Context.WebSite.SiteDb().Code.GetByNameOrId(Setting.CallbackCodeName);
            if (codeScript == null) throw new Exception("Not handle callback script code!");
            query.TryGetValue("code", out var code);
            if (string.IsNullOrWhiteSpace(code.ToString())) throw new Exception("code can't be empty");
            var redirectUrl = $"{Context.WebSite.BaseUrl()}_api/oauth2callback/{nameof(GoogleLogin)}";
            var tokenString = _webClient.UploadString($"https://oauth2.googleapis.com/token?client_id={Setting.Appid}&redirect_uri={redirectUrl}&client_secret={Setting.Secret}&code={code}&grant_type=authorization_code", "");
            var token = JsonHelper.Deserialize<Dictionary<string, object>>(tokenString.ToString());
            var userInfoString = _webClient.DownloadString($"https://www.googleapis.com/userinfo/v2/me?access_token={token["access_token"]}");
            var userInfo = JsonHelper.Deserialize<Dictionary<string, object>>(userInfoString);
            query.Add(nameof(token), token);
            query.Add(nameof(userInfo), userInfo);
            Context.Request.Body = JsonHelper.Serialize(query);
            return Manager.ExecuteCode(Context, codeScript.Body, codeScript.Id);
        }
    }
}
