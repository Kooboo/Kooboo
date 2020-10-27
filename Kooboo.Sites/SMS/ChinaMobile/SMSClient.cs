using Kooboo.Sites.SMS.ChinaMobile.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.SMS.ChinaMobile
{
    public class SMSClient
    {
        private readonly string _baseUrl = "http://112.35.1.155:1992/sms/";
        private readonly HttpClient _httpClient;
        private readonly SMSConfig _config;

        public SMSClient(SMSConfig config)
        {
            _httpClient = new HttpClient();
            _config = config;
        }

        public async Task<SMSResult> SendAsync(IEnumerable<string> toPhoneNumbers, string content)
        {
            if (toPhoneNumbers == null ||
                !toPhoneNumbers.Any() ||
                toPhoneNumbers.Any(it => string.IsNullOrWhiteSpace(it)))
            {
                return SMSResult.Failure("ToPhoneNumbers is required.");
            }

            if (string.IsNullOrEmpty(content))
            {
                return SMSResult.Failure("Content is required.");
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Add("ContentType", Encoding.UTF8.HeaderName);

                var stringContent = TranslateToStringContent(toPhoneNumbers, content);

                var response = await _httpClient.PostAsync(_baseUrl + "norsubmit", stringContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return SMSResult.Failure($"SMS sending request failed. ResponseStatus: [{response.StatusCode}], Content: [{responseString}]");
                }

                var notificationSubmitResponse = JsonConvert.DeserializeObject<NotificationSubmitResponse>(responseString);
                if (!notificationSubmitResponse.Success)
                {
                    return SMSResult.Failure($"SMS sending failed. ResponseCode: [{notificationSubmitResponse.Rspcod}]");
                }

                return SMSResult.Success();
            }
            catch (Exception ex)
            {
                return SMSResult.Failure($"Exception occur: {JsonConvert.SerializeObject(ex)}");
            }
        }

        private StringContent TranslateToStringContent(IEnumerable<string> mobiles, string content)
        {
            var notificationSubmitRequest = new NotificationSubmitRequest(
                                _config.EcName,
                                _config.ApId,
                                _config.SecretKey,
                                string.Join(",", mobiles),
                                content,
                                _config.Sign);


            var requestString = JsonConvert.SerializeObject(notificationSubmitRequest);
            var requestBytes = Encoding.UTF8.GetBytes(requestString);
            var base64String = Convert.ToBase64String(requestBytes);

            return new StringContent(base64String);
        }
    }
}
