using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Kooboo.Lib.Helper.ApiClient;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public class DwollaApi
    {
        private readonly DwollaSetting Setting;
        private readonly string contentType = "application/vnd.dwolla.v1.hal+json";
        private string Token;

        public string ApiBaseAddress => Setting.IsUsingSanbox ? "https://api-sandbox.dwolla.com" : "https://api.dwolla.com";

        public string AuthBaseAddress => Setting.IsUsingSanbox ? "https://accounts-sandbox.dwolla.com" : "https://accounts.dwolla.com";

        public DwollaApi(DwollaSetting setting)
        {
            Setting = setting;
            GetToken();
        }

        public async void GetToken()
        {
            var client = new HttpClient();
            var headers = new Dictionary<string, string>();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, AuthBaseAddress + "/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"client_id", Setting.Key }, {"client_secret", Setting.Secret }, {"grant_type", "client_credentials"}
                })
            };
            var encode = Convert.ToBase64String(Encoding.GetEncoding("UTF-8").GetBytes(Setting.Key + ":" + Setting.Secret));
            headers.Add("Authorization", "Basic " + encode);
            headers.Add("Accept", contentType);
            var result = await client.SendAsync(requestMessage).Result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<TokenResponse>(result);
            Token = response.Token;
        }

        public async Task<ApiResponse> CreateCustomer(Customer customer)
        {
            var client = Create();
            var format = string.Format("{{\"firstName\": \"{0}\",\"lastName\": \"{1}\",\"email\": \"{2}\"}}", customer.FirstName, customer.LastName, customer.Email);
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + Token },
                { "Accept", contentType }
            };
            var result = await client.PostAsync(ApiBaseAddress + "/customers", format, contentType, headers);
            return result;
        }

        public async Task<IavTokenResponse> CreateIavToken(string customerAddress)
        {
            var client = Create("Bearer", Token);
            var headers = new Dictionary<string, string>
            {
                { "Accept", contentType }
            };
            var result = await client.PostAsync($"{customerAddress}/iav-token", "", contentType, headers);

            return JsonConvert.DeserializeObject<IavTokenResponse>(result.Content);
        }

        public async Task<TransferResponse> CreateTransfer(CreateTransferRequest request)
        {
            var client = Create("Bearer", Token);
            var headers = new Dictionary<string, string>
            {
                { "Accept", contentType },
                { "Idempotency-Key", Guid.NewGuid().ToString() }
            };
            var body = JsonConvert.SerializeObject(request);
            var result = await client.PostAsync($"{ApiBaseAddress}/transfers", body, contentType, headers);
            var transferResult = new TransferResponse();
            if (result.IsSuccessStatusCode)
            {
                transferResult.Status = result.StatusCode.ToString();
                transferResult.TransferURL = result.Headers.Location;
            }
            else
            {
                transferResult.Status = "failed";
            }
            return transferResult;
        }

        public async Task<TransferResponse> GetTransfer(string transfersUrl)
        {
            var client = Create("Bearer", Token);
            var headers = new Dictionary<string, string>
            {
                { "Accept", contentType }
            };
            var result = await client.GetAsync(transfersUrl, headers);
            var response = JsonConvert.DeserializeObject<TransferResponse>(result.Content);
            return response;
        }

        public async Task<bool> CreateWebhookSubscription(string endpoint, string webhookSecret)
        {
            var client = Create("Bearer", Token);
            var headers = new Dictionary<string, string>
            {
                { "Accept", contentType }
            };
            var body = string.Format("{{\"url\": \"{0}\",\"secret\": \"{1}\"}}", endpoint, webhookSecret);
            var response = await client.PostAsync($"{ApiBaseAddress}/webhook-subscriptions", body, contentType, headers);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<GetWebhookSubscriptionsResponse> GetWebhookSubscription()
        {
            var result = new GetWebhookSubscriptionsResponse();
            var client = Create("Bearer", Token);
            var headers = new Dictionary<string, string>
            {
                { "Accept", contentType }
            };
            var response = await client.GetAsync($"{ApiBaseAddress}/webhook-subscriptions", headers);
            if (response.IsSuccessStatusCode)
            {
                result = JsonConvert.DeserializeObject<GetWebhookSubscriptionsResponse>(response.Content);
                return result;
            }
            return result;
        }

        public async Task<bool> DeleteWebhookSubscriptions(string id)
        {
            var client = Create("Bearer", Token);
            var headers = new Dictionary<string, string>
            {
                { "Accept", contentType }
            };

            var response = await client.DeleteAsync($"{ApiBaseAddress}/webhook-subscriptions/" + id, headers);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
