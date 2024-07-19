using System.Linq;
using System.Web;
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty.Providers;

internal sealed class CdnjsProvider : IThirdPartyProvider<OnlinePackageItemViewModel>
{
    private readonly string fileType;
    private readonly ExistsFetcher existsFetcher;
    private readonly NameBuilder nameBuilder;

    public CdnjsProvider(string fileType, ExistsFetcher existsFetcher, NameBuilder nameBuilder = null)
    {
        this.fileType = fileType;
        this.existsFetcher = existsFetcher;
        this.nameBuilder = nameBuilder;
    }

    public InfiniteListViewModel<OnlinePackageItemViewModel> Search(SiteDb siteDb, string keyword, int page, int pageSize)
    {
        var response = new InfiniteListViewModel<OnlinePackageItemViewModel>
        {
            PageNr = 1,
            PageSize = pageSize,
        };
        var keywordArg = HttpUtility.UrlEncode(keyword);

        var url = $"https://api.cdnjs.com/libraries?search={keywordArg}&fields=version,fileType,description";
        var jsonString = HttpHelper.GetString(url);
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return response;
        }

        var exists = existsFetcher?.Invoke(siteDb) ?? Enumerable.Empty<string>();

        var data = JsonHelper.Deserialize<CdnjsResponse>(jsonString);
        response.List = QueryData(exists, data);
        response.PageSize = response.List.Count();
        response.HasMore = false; // 不支持分页，一次性全部加载

        return response;
    }

    private string GetFileName(CdnjsResult item)
    {
        return nameBuilder?.Invoke(item.name, item.version, item.fileType) ?? $"{StringHelper.ToValidFileName(System.IO.Path.GetFileNameWithoutExtension(item.name))}@{item.version}.{item.fileType}";
    }

    private IEnumerable<OnlinePackageItemViewModel> QueryData(IEnumerable<string> exists, CdnjsResponse data)
    {
        foreach (var it in data.results)
        {
            if (it.fileType != fileType)
            {
                continue;
            }

            var name = GetFileName(it);
            yield return new OnlinePackageItemViewModel
            {
                Id = it.name.ToHashGuid(),
                Name = name,
                PackageName = it.name,
                FullUrl = it.latest,
                Type = it.fileType,
                Description = it.description,
                Version = it.version,
                Installed = exists.Contains(name, StringComparer.OrdinalIgnoreCase)
            };
        }
    }

    private class CdnjsResponse
    {
        public List<CdnjsResult> results { get; set; }
    }

    private class CdnjsResult
    {
        public string name { get; set; }

        public string latest { get; set; }

        public string version { get; set; }

        public string fileType { get; set; }

        public string description { get; set; }
    }
}
