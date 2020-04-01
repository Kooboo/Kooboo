using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Kooboo.Lib.Helper;
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

        public WebhooksEvent Unmarshal(byte[] postData, NameValueCollection requestHeaders)
        {
            Validate(postData, requestHeaders);
            var body = Encoding.UTF8.GetString(postData);
            WebhooksEvent unmarshalledEvent = JsonConvert.DeserializeObject<WebhooksEvent>(body);
            return unmarshalledEvent;
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


        protected void Validate(byte[] postData, NameValueCollection requestHeaders)
        {
            var numberOfSignatureHeaders = requestHeaders.GetValues("X-GCS-Signature");

            if (numberOfSignatureHeaders.Count() == 0)
            {
                throw new Exception("Missing X-GCS-Signature header");
            }
            if (numberOfSignatureHeaders.Count() != 1)
            {
                throw new Exception("Duplicate X-GCS-Signature header");
            }

            var numberOfKeyIdHeaders = requestHeaders.GetValues("X-GCS-KeyId");

            if (numberOfKeyIdHeaders.Count() == 0)
            {
                throw new Exception("Missing X-GCS-KeyId header");
            }
            if (numberOfKeyIdHeaders.Count() != 1)
            {
                throw new Exception("Duplicate X-GCS-KeyId header");
            }

            var signature = numberOfSignatureHeaders[0];

            using (var mac = new HMACSHA256(StringUtils.Encoding.GetBytes(setting.SecretKey)))
            {
                mac.Initialize();
                byte[] unencodedResult = mac.ComputeHash(postData);
                var expectedSignature = Convert.ToBase64String(unencodedResult);
                if (!signature.Equals(expectedSignature))
                {
                    throw new Exception("failed to validate signature '" + signature + "'");
                }
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
