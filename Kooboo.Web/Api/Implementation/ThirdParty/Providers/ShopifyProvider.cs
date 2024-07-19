using System.Web;
using Kooboo.Dom;
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty.Providers;

internal sealed class ShopifyProvider : IThirdPartyProvider<MediaFileViewModel>
{
    public InfiniteListViewModel<MediaFileViewModel> Search(SiteDb siteDb, string keyword, int page, int pageSize)
    {
        var response = new InfiniteListViewModel<MediaFileViewModel>
        {
            PageNr = page,
            PageSize = 0, // 不支持控制pageSize
        };
        var url = $"https://www.shopify.com/stock-photos/photos/search?page={page}&q={HttpUtility.UrlEncode(keyword)}";

        var client = HttpClientHelper.CreateClientInstance();

        var html = client.GetStringAsync(url).Result;

        if (string.IsNullOrWhiteSpace(html))
        {
            return response;
        }

        var doc = DomParser.CreateDom(html);
        response.List = QueryData(doc);
        response.HasMore = doc.Select(".pagination .next")?.length > 0;

        return response;
    }

    private static IEnumerable<MediaFileViewModel> QueryData(Document doc)
    {
        var images = doc.Select("#Main .section:first-child .grid .photo-card img");
        foreach (var image in images.item)
        {
            var srcSet = image.getAttribute("data-srcset");
            if (string.IsNullOrWhiteSpace(srcSet))
            {
                continue;
            }

            var src = image.getAttribute("src");
            if (string.IsNullOrWhiteSpace(src))
            {
                continue;
            }

            var uri = new Uri(src);
            var query = HttpUtility.ParseQueryString(uri.Query);
            var name = System.IO.Path.GetFileName(uri.LocalPath);

            yield return new MediaFileViewModel
            {
                Id = src.ToHashGuid(),
                Alt = image.getAttribute("alt"),
                Thumbnail = srcSet.Split(',')[0] ?? src,
                Url = src,
                Width = int.Parse(query.Get("width") ?? "0"),
                IsImage = true,
                Name = name,
                PreviewUrl = srcSet.Split(',')[1] ?? src,
                MimeTypeOverride = "image/jpeg"
            };
        }
    }
}