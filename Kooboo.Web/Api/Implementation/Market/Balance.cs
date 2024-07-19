using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Models.Market;
using Kooboo.Lib.Helper;

namespace Kooboo.Web.Api.Implementation.Market
{
    public class Balance : IApi
    {
        public string ModelName => "Balance";

        public bool RequireSite => false;

        public bool RequireUser => true;

        public BalanceMode Current(ApiCall call)
        {
            var orgid = call.Context.User.CurrentOrgId;

            var org = GlobalDb.Organization.GetFromAccount(orgid);

            if (org != null)
            {
                return new BalanceMode() { Amount = org.Balance, Currency = org.Currency };
            }
            else
            {
                return new BalanceMode() { Amount = 0, Currency = "CNY" };
            }
        }


        //   public List<BalanceUsage> BalanceList(Guid OrgId, ApiCall call)
        public List<BalanceUsage> RecentList(ApiCall call)
        {

            var url = Kooboo.Data.Helper.AccountUrlHelper.Order("BalanceList");

            Dictionary<string, string> para = new Dictionary<string, string>();

            var result = HttpHelper.Get2<List<BalanceUsage>>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            if (result != null)
            {
                foreach (var item in result)
                {
                    item.Amount = Math.Round(item.Amount, 2);
                }
            }

            return result;
        }

    }



    public record BalanceMode
    {
        public double Amount { get; set; }

        public string Currency { get; set; }
    }
}
