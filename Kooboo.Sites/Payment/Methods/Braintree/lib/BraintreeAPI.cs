using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Kooboo.Lib.Helper;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class BraintreeAPI
    {
        private const string SALE = "sale";

        public BraintreeSetting setting;

        public const string TRANSACTION_SETTLED = "transaction_settled";
        public const string TRANSACTION_SETTLEMENT_DECLINED = "transaction_settlement_declined";

        public BraintreeAPI(BraintreeSetting setting)
        {
            this.setting = setting;
        }

        public TransactionResponse Sale(TransactionRequest request)
        {
            try
            {
                request.Transaction.Type = SALE;
                string response = Post(string.Format("{0}/merchants/{1}/transactions", setting.ServerUrl, setting.MerchantId),
                    JsonConvert.SerializeObject(request));
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<TransactionResponse>(response);

                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TransactionResponse Find(string id)
        {
            try
            {
                if (id != null || !id.Trim().Equals(""))
                {
                    string response = Get(string.Format("{0}/merchants/{1}/transactions/{2}", setting.ServerUrl, setting.MerchantId, id));

                    if (!string.IsNullOrEmpty(response))
                    {
                        var result = JsonConvert.DeserializeObject<TransactionResponse>(response);

                        return result;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Generate(ClientTokenRequest request = null)
        {
            if (request == null)
                request = new ClientTokenRequest()
                {
                    CientToken = new ClientToken
                    {
                        Version = "2"
                    }
                };
            try
            {
                string response = Post(string.Format("{0}/merchants/{1}/client_token", setting.ServerUrl, setting.MerchantId),
                            JsonConvert.SerializeObject(request));

                var result = JsonConvert.DeserializeObject<ClientTokenResponse>(response);
                return result.CientToken.value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Notification Parse(string signature, string payload)
        {
            ValidateSignature(signature, payload);
            var stringPayload = Encoding.GetEncoding(0).GetString(Convert.FromBase64String(payload));
            try
            {
                var res = JsonConvert.DeserializeObject<Notification>(stringPayload);

                if (string.Equals(res.kind, TRANSACTION_SETTLED, StringComparison.OrdinalIgnoreCase)
               || string.Equals(res.kind, TRANSACTION_SETTLEMENT_DECLINED, StringComparison.OrdinalIgnoreCase))
                {
                    return res;
                }

                return null;
            }
            catch (Exception)
            {

                throw new Exception(stringPayload);
            }
        }

        private void ValidateSignature(string signature, string payload)
        {
            if (signature == null)
            {
                throw new Exception("signature cannot be null");
            }
            if (payload == null)
            {
                throw new Exception("payload cannot be null");
            }
            var match = Regex.Match(payload, @"[^A-Za-z0-9+=/\n]");
            if (match.Success)
            {
                throw new Exception("payload contains illegal characters");
            }

            string matchingSignature = null;
            string[] signaturePairs = signature.Split('&');

            foreach (var signaturePair in signaturePairs)
            {
                if (signaturePair.IndexOf('|') >= 0)
                {
                    string[] candidatePair = signaturePair.Split('|');
                    if (setting.PublicKey.Equals(candidatePair[0]))
                    {
                        matchingSignature = candidatePair[1];
                        break;
                    }
                }
            }

            if (matchingSignature == null)
            {
                throw new Exception("no matching public key");
            }

            if (!(PayloadMatches(matchingSignature, payload) || PayloadMatches(matchingSignature, payload + "\n")))
            {
                throw new Exception("signature does not match payload - one has been modified");
            }
        }

        private bool PayloadMatches(string signature, string payload)
        {
            var sha1Hasher = new Sha1Hasher();
            string computedSignature = sha1Hasher.HmacHash(setting.PrivateKey, payload).ToLower();
            var crypto = new Crypto();
            return crypto.SecureCompare(computedSignature, signature);
        }

        private string Post(string url, string body)
        {
            var resp = ApiClient.CreateWithBasicAuth(setting.PublicKey, setting.PrivateKey, Encoding.Default)
                            .PostAsync(url,
                            body, headers: new Dictionary<string, string> {
                                { "Accept", "application/json" },
                                { "X-ApiVersion", "5" }
                            }).Result;
            if (!resp.IsSuccessStatusCode)
            {
                var content = JsonConvert.DeserializeObject<Dictionary<string, object>>(resp.Content);
                var apiErrorResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(content["apiErrorResponse"].ToString());
                throw new Exception(apiErrorResponse["message"].ToString());
            }

            return resp.Content;

        }

        private string Get(string url)
        {
            var resp = ApiClient.CreateWithBasicAuth(setting.PublicKey, setting.PrivateKey, Encoding.Default)
                            .GetAsync(url, new Dictionary<string, string> {
                                { "Accept", "application/json" },
                                { "X-ApiVersion", "5" }
                            }).Result;
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception("找不到订单！");
            }

            return resp.Content;

        }
    }
}
