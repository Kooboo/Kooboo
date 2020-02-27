using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.Payment.Methods.Mollie.Lib
{
    public class MollieApi
    {
        public static async Task<MollieResponse> CreatePayment(MolliePaymentRequest molliePaymentRequest, string apiToken)
        {
            var body = await Post("https://api.mollie.com/v2/payments", JsonHelper.Serialize(molliePaymentRequest), apiToken);
            return JsonHelper.Deserialize<MollieResponse>(body);
        }

        public static async Task<MollieResponse> CheckStatus(string paymentId, string apiToken)
        {
            var body = await Get($"https://api.mollie.com/v2/payments/{paymentId}", apiToken);
            return JsonHelper.Deserialize<MollieResponse>(body);
        }

        public static async Task<string> Post(string url, string jsonBody, string bearerToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                var resp = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));
                return await ReadResponse(resp);
            }
        }

        public static async Task<string> Get(string url, string bearerToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                var resp = await client.GetAsync(url);
                return await ReadResponse(resp);
            }
        }

        private static async Task<string> ReadResponse(HttpResponseMessage resp)
        {
            var response = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                return response;
            }

            var error = JsonHelper.Deserialize<ErrorResponse>(response);
            throw new HttpRequestException($"Error Status: {error.Status}; filed: {error.Field}; detail: {error.Detail}; link: {error.Links.Documentation.Href}.");
        }
    }
}