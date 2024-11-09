using System.Linq;
using Kooboo.Api;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Data.Permission;
using Kooboo.Data;
using System.Text.Json;
using Kooboo.Api.ApiResponse;
using Kooboo.Sites.Commerce;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Payment;
using Kooboo.Sites.Commerce.Notification;
using Kooboo.Data.Storage;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class CommerceSettingsApi : CommerceApi
    {
        public override string ModelName => "CommerceSettings";

        [Permission(Feature.COMMERCE_SETTINGS, Action = Data.Permission.Action.VIEW)]
        public JsonTextResponse Currencies(ApiCall apiCall)
        {
            var json = ResourceService.Currencies();
            return new JsonTextResponse(json);
        }

        [Permission(Feature.COMMERCE_SETTINGS, Action = Data.Permission.Action.VIEW)]
        public KeyValuePair<string, string>[] Payments(ApiCall apiCall)
        {
            return PaymentContainer.PaymentMethods.Select(s => new KeyValuePair<string, string>(s.Name, s.DisplayName)).ToArray();
        }

        [Permission(Feature.COMMERCE_SETTINGS, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_PRODUCT_TYPE, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
        public Settings Get(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            return commerce.Settings;
        }

        [Permission(Feature.COMMERCE_SETTINGS, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<Settings>(body, Defaults.JsonSerializerOptions);
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(apiCall.WebSite.Id + "settings"),
                Name = $"Commerce settings",
                Body = JsonSerializer.Serialize(model, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Settings
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_SETTINGS, Action = Data.Permission.Action.VIEW)]
        public EmailEvent[] EmailEvents(ApiCall call)
        {
            return NotificationService.GetEmailEvents(call.Context);
        }

        [Permission(Feature.COMMERCE_SETTINGS, Action = Data.Permission.Action.VIEW)]
        public WebhookEvent[] WebhookEvents(ApiCall call)
        {
            return NotificationService.GetWebhookEvents(call.Context);
        }

        [Permission(Feature.COMMERCE_SETTINGS, Action = Data.Permission.Action.VIEW)]
        public EmailNotification EmailPreview(ApiCall call)
        {
            var emailNotification = JsonSerializer.Deserialize<EmailNotification>(call.Context.Request.Body, Defaults.JsonSerializerOptions);
            return NotificationService.RenderEmail(call.WebSite, emailNotification);
        }

        [Permission(Feature.COMMERCE_SETTINGS, Action = Data.Permission.Action.VIEW)]
        public PagedList<EmailLog> EmailLogs(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var pager = ApiHelper.GetPager(apiCall, 10);
            var result = commerce.EmailLog.Query(pager);
            var events = EmailEvents(apiCall);
            foreach (var item in result.DataList)
            {
                var @event = events.FirstOrDefault(f => f.Name == item.Event);
                item.Event = @event.Display;
            }

            return result;
        }

        [Permission(Feature.COMMERCE_SETTINGS, Action = Data.Permission.Action.VIEW)]
        public PagedList<WebhookLog> WebhookLogs(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var pager = ApiHelper.GetPager(apiCall, 10);
            var result = commerce.WebhookLog.Query(pager);
            var events = WebhookEvents(apiCall);
            foreach (var item in result.DataList)
            {
                var @event = events.FirstOrDefault(f => f.Name == item.Event);
                item.Event = @event.Display;
            }
            return result;
        }
    }
}
