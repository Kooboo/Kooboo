using Kooboo.Api;
using Kooboo.Api.ApiResponse;

namespace Kooboo.Web.Api.V2
{
    public class LightHouseReportApi : IApi
    {
        public string ModelName => "LightHouse";

        public bool RequireSite => false;

        public bool RequireUser => false;


        public void ReadReport(Guid Id, ApiCall call)
        {
            Kooboo.Sites.Scripting.Global.WebUtility.Google google = new Sites.Scripting.Global.WebUtility.Google(call.Context);

            var json = google.GetJson(Id);

            if (json != null)
            {

                string url = "/_Admin/LightHouse/index.html?jsonurl=/_api/lighthouse/JsonFile?id=" + Id.ToString();

                call.Context.Response.Redirect(302, url);
            }


            // return "/_Admin/LightHouse/index.html?gzip=1#" + GZipBase64; 
            ///return reportViewer + "?jsonurl=" + jsonPath;

        }

        public PlainResponse JsonFile(Guid Id, ApiCall call)
        {
            Kooboo.Sites.Scripting.Global.WebUtility.Google google = new Sites.Scripting.Global.WebUtility.Google(call.Context);

            var json = google.GetJson(Id);

            if (json != null)
            {
                PlainResponse res = new PlainResponse();
                res.Content = json;
                res.ContentType = "application/json";

                return res;
            }

            return new PlainResponse();

            // return "/_Admin/LightHouse/index.html?gzip=1#" + GZipBase64; 
            ///return reportViewer + "?jsonurl=" + jsonPath;

        }

    }
}
