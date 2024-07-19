using System.Linq;
using System.Web;
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty.Providers;

internal sealed class UnsplashProvider : IThirdPartyProvider<MediaFileViewModel>
{
    public InfiniteListViewModel<MediaFileViewModel> Search(SiteDb siteDb, string keyword, int page, int pageSize)
    {
        var response = new InfiniteListViewModel<MediaFileViewModel>
        {
            PageNr = page,
            PageSize = pageSize,
        };
        var url = $"https://unsplash.com/napi/search/photos?query={HttpUtility.UrlEncode(keyword)}&per_page={pageSize}&page={page}";
        var jsonString = HttpHelper.GetString(url);
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return response;
        }

        var data = JsonHelper.Deserialize<UnsplashResponse>(jsonString);
        response.List = QueryData(data);
        response.HasMore = page < data.total_pages;
        return response;
    }

    private static IEnumerable<MediaFileViewModel> QueryData(UnsplashResponse data)
    {
        return data.results.Select(it => new MediaFileViewModel
        {
            Id = it.id.ToHashGuid(),
            Alt = it.alt_description,
            Thumbnail = it.urls?.thumb,
            Url = it.urls?.regular,
            Height = it.height,
            Width = it.width,
            IsImage = true,
            LastModified = it.updated_at,
            Name = $"{it.slug}.jpg",
            PreviewUrl = it.urls.regular,
            MimeTypeOverride = "image/jpeg"
        });
    }

    private class UnsplashResponse
    {
        public int total { get; set; }
        public int total_pages { get; set; }
        public List<UnsplashResult> results { get; set; }
    }

    private class UnsplashUrls
    {
        //public string raw { get; set; }
        //public string full { get; set; }
        public string regular { get; set; }
        //public string small { get; set; }
        public string thumb { get; set; }
        //public string small_s3 { get; set; }

    }

    private class UnsplashResult
    {
        public string id { get; set; }
        public string slug { get; set; }
        //public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        //public DateTime? promoted_at { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        //public string color { get; set; }
        //public string description { get; set; }
        public string alt_description { get; set; }
        public UnsplashUrls urls { get; set; }
    }
}
