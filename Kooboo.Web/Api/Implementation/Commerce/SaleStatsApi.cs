using System.IO;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class SaleStatsApi : CommerceApi
    {
        public override string ModelName => "SaleStats";

        [Permission(Feature.COMMERCE_SALE_STATS, Action = Data.Permission.Action.VIEW)]
        public ProductSaleStats ProductSaleStats(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<SaleStatsQuery>(body, Defaults.JsonSerializerOptions);
            var saleStatsService = new SaleStatsService(commerce);
            return saleStatsService.GetProductSaleStats(model);
        }

        [Permission(Feature.COMMERCE_SALE_STATS, Action = Data.Permission.Action.VIEW)]
        public OrderSaleStats OrderSaleStats(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<SaleStatsQuery>(body, Defaults.JsonSerializerOptions);
            var saleStatsService = new SaleStatsService(commerce);
            return saleStatsService.GetOrderSaleStats(model);
        }

        [Permission(Feature.COMMERCE_SALE_STATS, Action = Data.Permission.Action.VIEW)]
        public DailySaleStats DailySaleStats(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<SaleStatsQuery>(body, Defaults.JsonSerializerOptions);
            var saleStatsService = new SaleStatsService(commerce);
            return saleStatsService.GetDailySaleStats(model);
        }

        public record GenerateInfo(string File, string Name);
        [Permission(Feature.COMMERCE_SALE_STATS, Action = Data.Permission.Action.VIEW)]
        public GenerateInfo GenerateProductSaleStats(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<SaleStatsQuery>(body, Defaults.JsonSerializerOptions);
            var saleStatsService = new SaleStatsService(commerce);
            var file = saleStatsService.ExportProductSaleStats(model, apiCall.Context);
            return new GenerateInfo(file, model.GetFileName());
        }

        [Permission(Feature.COMMERCE_SALE_STATS, Action = Data.Permission.Action.VIEW)]
        public GenerateInfo GenerateOrderSaleStats(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<SaleStatsQuery>(body, Defaults.JsonSerializerOptions);
            var saleStatsService = new SaleStatsService(commerce);
            var file = saleStatsService.ExportOrderSaleStats(model, apiCall.Context);
            return new GenerateInfo(file, model.GetFileName());
        }

        [Permission(Feature.COMMERCE_SALE_STATS, Action = Data.Permission.Action.VIEW)]
        public GenerateInfo GenerateDailySaleStats(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<SaleStatsQuery>(body, Defaults.JsonSerializerOptions);
            var saleStatsService = new SaleStatsService(commerce);
            var file = saleStatsService.ExportDailySaleStats(model, apiCall.Context);
            return new GenerateInfo(file, model.GetFileName());
        }

        [Permission(Feature.COMMERCE_SALE_STATS, Action = Data.Permission.Action.VIEW)]
        public virtual BinaryResponse ExportSaleStats(ApiCall call)
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
    }
}
