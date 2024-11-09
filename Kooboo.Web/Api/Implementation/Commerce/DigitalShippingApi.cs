using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class DigitalShippingApi : CommerceApi
    {
        public override string ModelName => "digitalShipping";

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.EDIT)]
        public void Create(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<DigitalShippingCreate>(body, Defaults.JsonSerializerOptions);
            var shipping = model.ToShipping();
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(shipping.Id),
                Name = $"DigitalShipping:{model.Name}",
                Body = JsonSerializer.Serialize(shipping, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.DigitalShipping
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.VIEW)]
        public DigitalShipping[] List(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var shippings = commerce.DigitalShipping.Entities;
            return shippings.ToArray();
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.EDIT)]
        public void Deletes(string[] ids, ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            foreach (var id in ids)
            {
                var guid = Lib.Security.Hash.ComputeHashGuid(id);
                var exist = siteDb.CommerceData.Get(guid);
                if (exist == default)
                {
                    commerce.DigitalShipping.Delete(d => d.Id == id);
                }
                else
                {
                    siteDb.CommerceData.Delete(guid, apiCall.Context.User.Id);
                }
            }
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<DigitalShippingEdit>(body, Defaults.JsonSerializerOptions);
            var entity = commerce.DigitalShipping.Entities.FirstOrDefault(g => g.Id == model.Id);
            model.UpdateShipping(entity);
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(entity.Id),
                Name = $"DigitalShipping:{model.Name}",
                Body = JsonSerializer.Serialize(entity, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.DigitalShipping
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.VIEW)]
        public DigitalShippingEdit GetEdit(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var entity = commerce.DigitalShipping.Entities.FirstOrDefault(g => g.Id == id);
            if (entity == null)
            {
                return new DigitalShippingEdit
                {
                    Name = string.Empty,
                    Description = string.Empty,
                    MailTemplate = ShippingMail.Default,
                    MailServerType = "kooboo",
                    CustomMailServer = new Mail.Models.SmtpSetting
                    {
                        Password = string.Empty,
                        Port = 587,
                        Server = string.Empty,
                        SSL = false,
                        UserName = string.Empty
                    }
                };
            }
            return new DigitalShippingEdit(entity);
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
        public DigitalShipping Get(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var shipping = commerce.DigitalShipping.Entities.FirstOrDefault(c => c.Id == id);
            if (shipping == default) shipping = commerce.DigitalShipping.Entities.FirstOrDefault(f => f.IsDefault);
            return shipping;
        }

        public void SetDefault(string id, ApiCall call)
        {
            var commerce = GetSiteCommerce(call);
            var shipping = commerce.DigitalShipping.Entities.FirstOrDefault(f => f.Id == id);
            if (shipping == null || shipping.IsDefault) return;
            var oldDefaults = commerce.DigitalShipping.Entities.Where(f => f.IsDefault).ToArray();

            foreach (var item in oldDefaults)
            {
                if (item.IsDefault)
                {
                    item.IsDefault = false;
                    commerce.DigitalShipping.AddOrUpdate(item);
                }
            }

            shipping.IsDefault = true;
            commerce.DigitalShipping.AddOrUpdate(shipping);
        }

        public IResponse Download(ApiCall call)
        {
            var orderId = call.Command.Value;
            var commerce = GetSiteCommerce(call);
            var order = commerce.Order.Get(o => o.Id == orderId);
            if (order == null)
            {
                return new PlainResponse { Content = "Order not found" };
            }

            if (!order.Paid)
            {
                return new PlainResponse { Content = "Order not paid" };
            }

            if (order.Canceled)
            {
                return new PlainResponse { Content = $"Order is canceled, reason:{order.CancelReason}" };
            }

            var digitalItemId = call.Context.Request.Path.Split('/').Last();
            OrderLine orderLine = null;
            DigitalOrderItem digitalItem = null;

            foreach (var line in order.Lines)
            {
                var found = line.DigitalItems.FirstOrDefault(f => f.Id == digitalItemId);
                if (found != null)
                {
                    orderLine = line;
                    digitalItem = found;
                    break;
                }
            }

            if (digitalItem == null)
            {
                return new PlainResponse { Content = "File not found" };
            }

            if (orderLine.MaxDownloadCount.HasValue)
            {
                if (digitalItem.DownloadCount >= orderLine.MaxDownloadCount * orderLine.TotalQuantity)
                {
                    return new PlainResponse { Content = "The number of downloads exceeded the limit" };
                }
                else
                {
                    digitalItem.DownloadCount++;
                    commerce.Order.AddOrUpdate(order);
                }
            }

            switch (digitalItem.Type)
            {
                case "file":
                    var filePath = Path.Combine(commerce.RootPath, "digitalFiles", digitalItem.Value, digitalItem.Name);
                    if (!File.Exists(filePath))
                    {
                        return new PlainResponse { Content = "File not found" };
                    }
                    var fs = File.OpenRead(filePath);
                    var fileName = WebUtility.UrlEncode(digitalItem.Name);
                    call.Context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                    return new BinaryResponse
                    {
                        Stream = fs,
                        ContentType = "application/octet-stream",
                    };

                case "link":
                    return new MetaResponse
                    {
                        StatusCode = 302,
                        RedirectUrl = digitalItem.Value,
                    };
                case "text":
                    return new PlainResponse { Content = $"{digitalItem.Name}:{digitalItem.Value}" };
                default:
                    return null;
            }

        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.VIEW)]
        public ShippingMail Preview(ShippingMail mail, ApiCall call)
        {
            return DigitalShippingService.RenderEmail(call.WebSite, mail, null);
        }
    }
}