using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using Kooboo.Lib.Helper;
using Kooboo.Data.Helper;

namespace Kooboo.Data.Repository
{
    public class CurrencyRepository : RepositoryBase<Currency>
    {
        private string ChangeCurrencyUrl = AccountUrlHelper.Currency("changeCurrency");
        private string ListUrl = AccountUrlHelper.Currency("list");
        private string PriceConvertUrl = AccountUrlHelper.Currency("priceConvert");

        public bool ChangeCurrency(string currencyCode,Guid userId)
        {
            var dic = new Dictionary<string,string>();
            dic.Add("currencyCode", currencyCode);
            dic.Add("userId", userId.ToString());
            
            var result= HttpHelper.Post<bool>(ChangeCurrencyUrl, dic);

            GlobalDb.Organization.RemoveOrgCache(userId);

            return result;
        }
        public List<Currency> List()
        {
            return Lib.Helper.HttpHelper.Get<List<Currency>>(ListUrl);
        }

        public decimal PriceConvert(decimal price,string fromCode,string toCode)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("price", price.ToString());
            dic.Add("fromCode", fromCode);
            dic.Add("toCode", toCode);

            return HttpHelper.Post<decimal>(PriceConvertUrl, dic);
        }
    }
}
