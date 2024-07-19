using Kooboo.Api;
using Kooboo.Data.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class CDNApi : IApi
    {
        public string ModelName => "CDN";

        public bool RequireSite => false;

        public bool RequireUser => true;

        public List<CDNViewModel> List(ApiCall call)
        {
            var url = Kooboo.Data.Helper.AccountUrlHelper.CDN("ListByOrganization");

            Dictionary<string, string> para = new Dictionary<string, string>();
            return Lib.Helper.HttpHelper.Get2<List<CDNViewModel>>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
                );
        }

        public void Update(Data.ViewModel.CDNViewModel model, ApiCall call)
        {
            var url = Kooboo.Data.Helper.AccountUrlHelper.CDN("Update");

            var json = System.Text.Json.JsonSerializer.Serialize(model);

            var ok = Lib.Helper.HttpHelper.Post<bool>(
                url,
                json,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context));
        }

    }
}
