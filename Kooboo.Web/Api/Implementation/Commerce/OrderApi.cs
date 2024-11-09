using System.IO;
using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class OrderApi : CommerceApi
    {
        public override string ModelName => "Order";

        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.EDIT)]
        public void Create(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<OrderCreate>(body, Defaults.JsonSerializerOptions);
            var orderService = new OrderService(commerce, apiCall.Context);
            orderService.Create(model);
        }

        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.VIEW)]
        public PagingResult List(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<OrderQuery>(body, Defaults.JsonSerializerOptions);
            return new OrderService(commerce, apiCall.Context).List(model);
        }


        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.VIEW)]
        public OrderDetail GetDetail(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            return new OrderService(commerce, apiCall.Context).GetDetail(id);
        }

        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.EDIT)]
        public void Cancel(string id, string reason, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            new OrderService(commerce, apiCall.Context).Cancel(id, reason);
        }

        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.EDIT)]
        public void Pay(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<OrderPay>(body, Defaults.JsonSerializerOptions);
            new OrderService(commerce, apiCall.Context).Pay(model.OrderId, model.Method);
        }

        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.EDIT)]
        public void Delivery(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<OrderDelivery>(body, Defaults.JsonSerializerOptions);
            var service = new OrderService(commerce, apiCall.Context);
            var order = service.GetDetail(model.Id);
            if (order == null) return;
            foreach (var item in model.Items)
            {
                service.UpdateShippingInfo(order.Id, item.Id, new DeliveryOptions
                {
                    ShippingCarrier = item.ShippingCarrier,
                    TrackingNumber = item.TrackingNumber,
                    DigitalItems = item.DigitalItems?.Select(s => s.ToDigitalOrderItem()).ToArray()
                });
            }
            service.Delivery(order.Id, model.Items.Select(s => s.Id).ToArray());
        }

        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.DELETE)]
        public void Deletes(string[] ids, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            commerce.Order.Delete(d => ids.Contains(d.Id));
        }

        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.VIEW)]
        public virtual BinaryResponse ExportExcel(ApiCall call)
        {
            var exportfile = call.GetValue("exportfile");
            var name = call.GetValue("name");
            var path = Path.Combine(AppSettings.TempDataPath, exportfile);

            if (File.Exists(path))
            {
                var response = new BinaryResponse
                {
                    ContentType = "application/octet-stream"
                };
                response.Headers.Add("Content-Disposition", $"attachment;filename={name}.xlsx");
                response.Stream = new FileStream(path, FileMode.Open);
                return response;
            }
            return null;
        }

        public record GenerateInfo(string File, string Name);
        [Permission(Feature.COMMERCE_ORDERS, Action = Data.Permission.Action.VIEW)]
        public GenerateInfo GenerateOrderExcel(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<OrderQuery>(body, Defaults.JsonSerializerOptions);
            var orderService = new OrderService(commerce, apiCall.Context);
            var file = orderService.ExportExcel(model, apiCall.Context);
            return new GenerateInfo(file, model.GetFileName());
        }
    }
}
