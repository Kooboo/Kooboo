using Kooboo.Api;
using Kooboo.Data.Helper;

namespace Kooboo.Web.Api.V2
{
    [ApiVersion(ApiVersion.V2)]
    public class PasswordApi : IApi
    {
        public string ModelName => "Password";

        public bool RequireSite => false;

        public bool RequireUser => false;

        public void SmsCode(string Tel, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("Tel", Tel);
            Lib.Helper.HttpHelper.Post<string>(AccountUrlHelper.Password("SmsCode"), para, throwError: true);
        }

        public void EmailCode(string email, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("email", email);
            Lib.Helper.HttpHelper.Post<string>(AccountUrlHelper.Password("EmailCode"), para, throwError: true);
        }

        public object ResetByEmail(string email, string code, string newPassword, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("email", email);
            para.Add("code", code);
            para.Add("newPassword", newPassword);
            return Lib.Helper.HttpHelper.Post<Dictionary<string, string>>(AccountUrlHelper.Password("ResetByEmail"), para, throwError: true);
        }

        public object ResetByTel(string Tel, string code, string newPassword, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("Tel", Tel);
            para.Add("code", code);
            para.Add("newPassword", newPassword);
            return Lib.Helper.HttpHelper.Post<Dictionary<string, string>>(AccountUrlHelper.Password("ResetByTel"), para, throwError: true);
        }
    }
}

