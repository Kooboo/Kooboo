using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.qualpay.lib
{
    public class QualpayAPI
    {
        public static Dictionary<string, string> CheckOutUrl(Dictionary<string, object> request, QualpayFormSetting setting)
        {
            try
            {
                request.Add("checkout_profile_id", setting.CheckoutProfileId);
                Validtion(request);
                var preferences = new Dictionary<string, string>
                {
                    { "success_url", setting.SuccessUrl },
                    { "failure_url", setting.FailureUrl }
                };

                request.Add("preferences", preferences);
                var body = JsonConvert.SerializeObject(request);
                var response = ApiClient.Create("Basic", setting.SecurityKey)
                    .PostAsync(setting.ServerUrl + "/platform/checkout", body)
                    .Result.EnsureSuccessStatusCode().Content;

                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                if (DataHelper.GetValue("code", response) != "0")
                {
                    throw new QualPayException(DataHelper.GetValue("message", response));
                }

                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(result["data"].ToString());

                CheckResult(data, request);

                return data;
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
            }

            return null;
        }

        private static void CheckResult(Dictionary<string, string> data, Dictionary<string, object> req)
        {
            if (data == null)
            {
                throw new QualPayException("API返回的支付链接为空！");
            }

            if (!string.Equals(data["amt_tran"], req["amt_tran"]))
            {
                throw new QualPayException("支付价格不一致！");
            }

            if (!string.Equals(data["tran_currency"], req["tran_currency"]))
            {
                throw new QualPayException("支付单位不一致！");
            }
        }

        private static void Validtion(IDictionary<string, object> data)
        {
            if (!data.ContainsKey("amt_tran"))
            {
                throw new QualPayException("amt_tran不能为空！");
            }

            if (!data.ContainsKey("tran_currency"))
            {
                throw new QualPayException("tran_currency不能为空！");
            }

            if (!data.ContainsKey("checkout_profile_id"))
            {
                throw new QualPayException("checkout_profile_id不能为空！");
            }
        }
    }
}
