using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Sites.Service;

namespace Kooboo.Web.Api
{
    public class I18NApi : IApi
    {
        public string ModelName => "I18N";

        public bool RequireSite => true;

        public bool RequireUser => false;

        public PlainResponse GetConfig(ApiCall call)
        {
            var all = call.GetBoolValue("all");
            var result = I18nService.GetConfig(call.Context, all);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            var body = JsonSerializer.Serialize(result, options);
            return new PlainResponse
            {
                Content = body,
                ContentType = "application/json",
            };
        }

        public void ChangeLocale(string locale, ApiCall call)
        {
            call.Context.Response.AppendCookie("lang", locale, DateTime.UtcNow.AddYears(10));
        }
    }
}
