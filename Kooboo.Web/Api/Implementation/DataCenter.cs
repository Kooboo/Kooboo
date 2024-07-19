using Kooboo.Api;
using Kooboo.Data.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class DataCenterApi : IApi
    {
        public string ModelName => "DataCenter";
        public bool RequireSite => false;
        public bool RequireUser => true;

        public List<DataCenterViewModel> List(ApiCall call)
        {
            var url = Data.Helper.AccountUrlHelper.DataCenter("list");

            Dictionary<string, string> para = new Dictionary<string, string>();
            var list = Lib.Helper.HttpHelper.Get2<List<DataCenterViewModel>>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context));

            var org = Data.GlobalDb.Organization.Get(call.Context.User.CurrentOrgId);

            if (org != null)
            {
                foreach (var item in list)
                {
                    if (item.Enable)
                    {
                        item.NavUrl = "https://" + Data.Helper.ServerDomainHelper.GetServerDomain(org.Name, item.Name, item.PrimaryDomain) + "/_admin/sites";
                    }
                }
            }

            foreach (var item in list)
            {
                var desc = item.Description;
                if (desc.StartsWith(item.Name))
                {
                    desc = desc.Substring(item.Name.Length).Trim();
                }

                var langDes = Data.Language.Hardcoded.GetValue(desc, call.Context);

                item.Description = langDes;
            }
            return list;
        }

        public bool Enable(string name, ApiCall call)
        {
            var url = Data.Helper.AccountUrlHelper.DataCenter("Enable");
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("name", name);

            var result = Lib.Helper.HttpHelper.Get2<bool>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            if (result)
            {
                return true;
            }
            else
            {
                throw new Exception(Kooboo.Data.Errors.ErrorCode.ErrorMemberLimited);
            }
        }
        public bool Disable(string name, ApiCall call)
        {
            var url = Data.Helper.AccountUrlHelper.DataCenter("Disable");

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("name", name);
            return Lib.Helper.HttpHelper.Get2<bool>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

        }

        public bool MakeDefault(string name, ApiCall call)
        {
            var url = Data.Helper.AccountUrlHelper.DataCenter("MakeDefault");

            Dictionary<string, string> para = new Dictionary<string, string>();
            // para.Add("OrgId", call.Context.User.CurrentOrgId.ToString());
            para.Add("name", name);
            return Lib.Helper.HttpHelper.Get2<bool>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

        }


        // account:   public DataCenterViewModel CurrentDC(ApiCall call)
        public string CurrentDC(ApiCall call)
        {
            var url = Data.Helper.AccountUrlHelper.DataCenter("CurrentDC");

            Dictionary<string, string> para = new Dictionary<string, string>();

            var dc = Lib.Helper.HttpHelper.Get2<DataCenterViewModel>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            if (dc != null)
            {
                return Data.Language.Hardcoded.GetValue(dc.Description, call.Context);
            }
            return null;
        }

    }
}
