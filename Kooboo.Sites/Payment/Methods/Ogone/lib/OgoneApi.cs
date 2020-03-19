using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kooboo.Sites.Payment.Methods.Ogone.lib
{
    public class OgoneApi
    {
        private const string SignatureString = "v1HMAC";
        private readonly OgoneSetting setting;

        public OgoneApi(OgoneSetting setting)
        {
            this.setting = setting;
        }

        public CreateHostedCheckoutResponse Hostedcheckouts(CreateHostedCheckoutRequest request)
        {
            try
            {
                string response = Post(string.Format("/v1/{0}/hostedcheckouts", setting.MerchantId),
                      JsonConvert.SerializeObject(request, new JsonSerializerSettings
                      {
                          ContractResolver = new DefaultContractResolver
                          {
                              NamingStrategy = new CamelCaseKeepFullCapsNamingStrategy()
                          },
                          NullValueHandling = NullValueHandling.Ignore
                      }));

                return JsonConvert.DeserializeObject<CreateHostedCheckoutResponse>(response);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public GetHostedCheckoutResponse GetHostedcheckouts(string hostedCheckoutId)
        {
            try
            {
                string response = Get(string.Format("/v1/{0}/hostedcheckouts/{1}", setting.MerchantId, hostedCheckoutId));

                return JsonConvert.DeserializeObject<GetHostedCheckoutResponse>(response);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string Get(string url)
        {
            var date = DateTime.UtcNow.ToString("r");
            var resp = ApiClient.Create("GCS", GetAuthorization(url, date, "", HttpMethod.Get))
                            .SendAsync(HttpMethod.Get, setting.ServerURL + url,
                             headers: new Dictionary<string, string>
                            {
                                { "Date", date }
                            }).Result;
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error Status: {resp.StatusCode}; content: {resp.Content}.");
            }

            return resp.Content;
        }

        private string Post(string url, string body)
        {
            var date = DateTime.UtcNow.ToString("r");
            string contentType = "application/json";
            var httpContent = new StringContent(body, null, contentType);
            httpContent.Headers.ContentType.CharSet = null;
            var resp = ApiClient.Create("GCS", GetAuthorization(url, date, contentType, HttpMethod.Post))
                            .SendAsync(HttpMethod.Post, setting.ServerURL + url,
                            httpContent, headers: new Dictionary<string, string>
                            {
                                { "Date", date }
                            }).Result;
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error Status: {resp.StatusCode}; content: {resp.Content}.");
            }

            return resp.Content;
        }

        private string GetAuthorization(string url, string date, string contentType, HttpMethod method)
        {
            var dataToSign = ToDataToSign(method, url, date, contentType);
            var authenticationSignature = SignatureString + ":" + setting.ApiKeyId + ":" + SignData(dataToSign);

            return authenticationSignature;
        }

        private string SignData(string theString)
        {
            var mac = new HMACSHA256(Encoding.UTF8.GetBytes(setting.SecretApiKey));
            mac.Initialize();
            byte[] unencodedResult = mac.ComputeHash(Encoding.UTF8.GetBytes(theString));
            var retVal = Convert.ToBase64String(unencodedResult);
            return retVal;
        }

        private string ToDataToSign(HttpMethod httpMethod, string url, string date, string contentType)
        {
            var sb = new StringBuilder();
            sb.AppendLLine(httpMethod.Method.ToUpperInvariant());
            sb.AppendLLine(contentType);
            sb.AppendLLine(date);
            sb.AppendLLine(url);
            return sb.ToString();
        }
    }
}
