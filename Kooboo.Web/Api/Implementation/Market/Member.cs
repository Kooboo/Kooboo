using Kooboo.Api;
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Data.Models.Market;
using Kooboo.Lib.Helper;

namespace Kooboo.Web.Api.Implementation.Market
{
    public class MemberApi : IApi
    {
        public string ModelName => "Member";

        public bool RequireSite => false;

        public bool RequireUser => true;

        public List<MemberOption> Options(ApiCall call)
        {
            var url = Kooboo.Data.Helper.AccountUrlHelper.Member("Options");
            var AuthHeader = Kooboo.Data.Helper.ApiHelper.GetAuthHeaders(call.Context);
            var options = HttpHelper.Get2<List<MemberOption>>(url, null, AuthHeader);

            Translate(options, call.Context);
            return options;
        }

        private void Translate(List<MemberOption> options, RenderContext context)
        {
            foreach (var item in options)
            {
                item.Title = Hardcoded.GetValue(item.Title, context);
                item.HeadLine = Hardcoded.GetValue(item.HeadLine, context);
                item.FunctionTitle = Hardcoded.GetValue(item.FunctionTitle, context);

                for (int i = 0; i < item.Functions.Count; i++)
                {
                    var func = item.Functions[i];
                    int firstChar = func.IndexOf(" ");
                    bool hasTranslation = false;
                    if (firstChar > -1)
                    {
                        var firstPart = func.Substring(0, firstChar);
                        if (int.TryParse(firstPart, out var value))
                        {
                            hasTranslation = true;
                            var remaining = func.Substring(firstChar + 1);
                            var translated = Hardcoded.GetValue(remaining, context);

                            var newText = firstPart + " " + translated;
                            item.Functions[i] = newText;
                        }
                    }

                    if (!hasTranslation)
                    {
                        int LastIndex = func.LastIndexOf(" ");
                        if (LastIndex > -1)
                        {
                            var LastPart = func.Substring(LastIndex + 1);
                            if (int.TryParse(LastPart, out var value))
                            {
                                hasTranslation = true;
                                var remaining = func.Substring(0, LastIndex);
                                var translated = Hardcoded.GetValue(remaining, context);

                                var newText = translated + " " + LastPart;
                                item.Functions[i] = newText;
                            }
                        }
                    }
                    if (!hasTranslation)
                    {
                        item.Functions[i] = Hardcoded.GetValue(func, context);
                    }
                }


            }
        }

        public System.Guid NewMemberShip(int servicelevel, ApiCall call)
        {
            if (servicelevel == 0)
            {
                throw new Exception("Free product, purchase not needed");
            }

            var url = Data.Helper.AccountUrlHelper.Order("MemberShip");

            url += "?OrgId=" + call.Context.User.CurrentOrgId;

            url += "&ServiceLevel=" + servicelevel.ToString();

            var res = HttpHelper.Get<OrderResponse>(url);

            if (res != null)
            {
                if (res.IsSuccess)
                {
                    return res.OrderId;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(res.Message))
                    {
                        throw new Exception(res.Message);
                    }
                }
            }

            return Guid.Empty;

        }


    }

}

