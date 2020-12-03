using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.OAuth2.Facebook;
using Kooboo.Sites.Scripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooboo.Sites.OAuth2.Google
{
    public class FacebookLogin : BaseOAuth2<FacebookSetting>
    {
        public FacebookLogin(RenderContext context) : base(context)
        {
        }

        [Description(@"
1.Config
site=>system=>settings=>FacebookLoginSetting

appid:xxx
secret:xxx
callbackCodeName:fbcallback
fields:

2.Create callbackCode script
development=>code=>create event code

name:fbcallback
code:
k.response.write(k.request.body)

3.Add page
<div>
    <script engine='kscript'>
        var url = k.oAuth2.facebook.getAuthUrl({
            state:'custom_state'
        })
    </script>
    <a k-href='url'>facebook login</a>
</div>
")]
        public override string GetAuthUrl(IDictionary<string, object> @params)
        {
            var redirectUrl = $"{Context.WebSite.BaseUrl()}_api/oauth2callback/{nameof(FacebookLogin)}";
            var url = $"https://www.facebook.com/v8.0/dialog/oauth?client_id={Setting.Appid}&redirect_uri={redirectUrl}";
            if (@params.TryGetValue("state", out var state)) url += $"&state={state}";
            if (@params.TryGetValue("scope", out var scope)) url += $"&scope={scope}";
            return url;
        }

        [Description(@"
1.Config
site=>system=>settings=>FacebookLoginSetting

appid:xxx
secret:xxx
callbackCodeName:fbcallback
fields:

2.Create callbackCode script
development=>code=>create event code

name:fbcallback
code:
k.response.write(k.request.body)

3.Add page
<div>
    <script engine='kscript'>
        var url = k.oAuth2.facebook.getAuthUrl()
    </script>
    <a k-href='url'>facebook login</a>
</div>
")]
        public string GetAuthUrl() => GetAuthUrl(new Dictionary<string, object>());

        [KIgnore]
        public override string Callback(IDictionary<string, object> query)
        {
            var codeScript = Context.WebSite.SiteDb().Code.GetByNameOrId(Setting.CallbackCodeName);
            if (codeScript == null) throw new Exception("Not handle callback script code!");
            query.TryGetValue("code", out var code);
            if (string.IsNullOrWhiteSpace(Setting.Fields)) Setting.Fields = "id,name,email,picture.type(large)";
            if (string.IsNullOrWhiteSpace(code.ToString())) throw new Exception("code can't be empty");
            var redirectUrl = $"{Context.WebSite.BaseUrl()}_api/oauth2callback/{nameof(FacebookLogin)}";
            var tokenString = _webClient.DownloadString($"https://graph.facebook.com/v8.0/oauth/access_token?client_id={Setting.Appid}&redirect_uri={redirectUrl}&client_secret={Setting.Secret}&code={code}");
            var token = JsonHelper.Deserialize<Dictionary<string, object>>(tokenString.ToString());
            var userInfoString = _webClient.DownloadString($"https://graph.facebook.com/v8.0/me?access_token={token["access_token"]}&fields={Setting.Fields}");
            var userInfo = JsonHelper.Deserialize<Dictionary<string, object>>(userInfoString);
            query.Add(nameof(token), token);
            query.Add(nameof(userInfo), userInfo);
            Context.Request.Body = JsonHelper.Serialize(query);
            return Manager.ExecuteCode(Context, codeScript.Body, codeScript.Id);
        }
    }
}
