using Kooboo.Api;
using Kooboo.Data.Helper;

namespace Kooboo.Web.Api.V2
{
    [ApiVersion(ApiVersion.V2)]
    public class VerifyCodeApi : IApi
    {
        public string ModelName => "VerifyCode";

        public bool RequireSite => false;

        public bool RequireUser => false;

        public void SmsCode(string Tel, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("Tel", Tel);
            Lib.Helper.HttpHelper.Post<string>(AccountUrlHelper.VerifyCode("SmsCode"), para, throwError: true);
        }

        public object VerifyTelCode(string Tel, int code, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("Tel", Tel);
            para.Add("code", code.ToString());
            return Lib.Helper.HttpHelper.Post<Dictionary<string, string>>(AccountUrlHelper.VerifyCode("VerifyTelCode"), para, throwError: true);
        }

        public object Bind(Guid verifyid, string UserName, string Password, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("verifyid", verifyid.ToString());
            para.Add("UserName", UserName);
            para.Add("Password", Password);
            return Lib.Helper.HttpHelper.Post<Dictionary<string, string>>(AccountUrlHelper.VerifyCode("Bind"), para, throwError: true);
        }

        public object Register(Guid verifyid, string UserName, string Password, ApiCall call)
        {
            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("verifyid", verifyid.ToString());
            para.Add("UserName", UserName);
            para.Add("Password", Password);
            return Lib.Helper.HttpHelper.Post<Dictionary<string, string>>(AccountUrlHelper.VerifyCode("Register"), para, throwError: true);
        }
    }
}

