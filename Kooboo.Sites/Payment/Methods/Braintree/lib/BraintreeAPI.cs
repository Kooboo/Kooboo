using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class BraintreeAPI
    {
        private const string SALE = "sale";

        private readonly BraintreeService Service;

        public BraintreeSetting setting;

        public BraintreeAPI(BraintreeSetting setting)
        {
            this.setting = setting;
            Service = new BraintreeService(setting);
        }

        public virtual TransactionResponse Sale(TransactionRequest request)
        {
            try
            {
                request.Transaction.Type = SALE;
                string response = Post(string.Format("{0}/merchants/{1}/transactions", setting.ServerUrl, setting.MerchantId),
                    JsonConvert.SerializeObject(request));

                var result = JsonConvert.DeserializeObject<TransactionResponse>(response);

                return result;
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

        public virtual Notification Parse(string signature, string payload)
        {
            ValidateSignature(signature, payload);
            var stringPayload = Encoding.GetEncoding(0).GetString(Convert.FromBase64String(payload));
            try
            {
                var res = JsonConvert.DeserializeObject<Notification>(stringPayload);
                return res;
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
            if (resp.IsSuccessStatusCode)
            {
                var content = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp.Content);
                var apiErrorResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(content["apiErrorResponse"]);
                throw new Exception(apiErrorResponse["message"]);
            }

            return resp.Content;

        }
    }
}
