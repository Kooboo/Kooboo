using Kooboo.Api;
using Kooboo.Data.Helper;

namespace Kooboo.Web.Api.V2
{
    [ApiVersion(ApiVersion.V2)]
    public class OAuthApi : IApi
    {
        public string ModelName => "OAuth";

        public bool RequireSite => false;

        public bool RequireUser => false;

        public string Url(string redirectUri, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("redirectUri", redirectUri);

            return Lib.Helper.HttpHelper.Post<string>(AccountUrlHelper.OAuth($"AuthUrl/{call.Command.Value}"), para);
        }

        public object Bind(Guid verifyid, string UserName, string Password, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("OAuthId", verifyid.ToString());
            para.Add("UserName", UserName);
            para.Add("Password", Password);
            return Lib.Helper.HttpHelper.Post<Dictionary<string, string>>(AccountUrlHelper.OAuth("Bind"), para);
        }

        public object Register(Guid verifyid, string UserName, string Password, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("OAuthId", verifyid.ToString());
            para.Add("UserName", UserName);
            para.Add("Password", Password);
            return Lib.Helper.HttpHelper.Post<Dictionary<string, string>>(AccountUrlHelper.OAuth("Register"), para);
        }
    }
}

