using System.Linq;
using Kooboo.Data.Cache;
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty.Providers;

internal sealed class BootCDNProvider : IThirdPartyProvider<OnlinePackageItemViewModel>
{
    private readonly string fileType;
    private readonly ExistsFetcher existsFetcher;
    private readonly NameBuilder nameBuilder;

    public BootCDNProvider(string fileType, ExistsFetcher existsFetcher, NameBuilder nameBuilder = null)
    {
        this.fileType = fileType;
        this.existsFetcher = existsFetcher;
        this.nameBuilder = nameBuilder;
    }


    public InfiniteListViewModel<OnlinePackageItemViewModel> Search(SiteDb siteDb, string keyword, int page, int pageSize)
    {
        var response = new InfiniteListViewModel<OnlinePackageItemViewModel>
        {
            PageNr = page,
            PageSize = pageSize,
        };

        if (string.IsNullOrWhiteSpace(keyword))
        {
            return response;
        }

        var exists = existsFetcher?.Invoke(siteDb) ?? Enumerable.Empty<string>();

        var data = QueryData(exists, keyword).Skip((response.PageNr - 1) * pageSize).Take(pageSize + 1).ToList();
        response.List = data.Take(pageSize);
        response.HasMore = data.Count > pageSize;

        return response;
    }

    private IEnumerable<OnlinePackageItemViewModel> QueryData(IEnumerable<string> exists, string keyword)
    {
        var list = ThirdPartyCache.Instance.GetOrCreate(nameof(BootCDNProvider), () =>
        {
            var data = FetchPackages().ToList();
            return data;
        }, new MemoryCacheBase.Options
        {
            AbsoluteExpiration = TimeSpan.FromHours(12)
        });

        var keywords = keyword.Split(' ').Distinct();
        foreach (var item in list)
        {
            var ext = System.IO.Path.GetExtension(item.FullUrl).TrimStart('.');
            if (ext != fileType)
            {
                continue;
            }

            var summary = $"{item.Name}|{item.PackageName}|{item.Description}|{item.Version}";

            if (!keywords.All(k => summary.Contains(k, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var package = item;

            package.Installed = exists.Contains(package.Name);

            yield return package;
        }
    }

    private string GetFileName(string name, string version, string type)
    {
        var ext = $".{type}";
        if (name.EndsWith(ext))
        {
            name = name.Replace(ext, string.Empty);
        }

        return nameBuilder?.Invoke(name, version, type) ?? $"{StringHelper.ToValidFileName(name)}@{version}.{type}";
    }

    private IEnumerable<OnlinePackageItemViewModel> FetchPackages()
    {
        var libsJson = HttpHelper.GetString("https://api.bootcdn.cn/libraries");
        if (string.IsNullOrWhiteSpace(libsJson))
        {
            yield break;
        }

        var infoJson = HttpHelper.GetString("https://api.bootcdn.cn/libs.min.json");
        if (string.IsNullOrEmpty(infoJson))
        {
            yield break;
        }

        var infosData = JsonHelper.Deserialize<IEnumerable<string[]>>(infoJson);
        var infoMaps = new Dictionary<string, string>();
        foreach (var info in infosData)
        {
            if (info.Length != 2)
            {
                continue;
            }
            infoMaps[info[0]] = info[1];
        }

        var libsData = JsonHelper.Deserialize<BootCDNResponse>(libsJson);
        var empty = new List<string>();
        foreach (var it in libsData.results)
        {
            if (string.IsNullOrEmpty(it.latest))
            {
                empty.Add(it.name);
                continue;
            }
            var uri = new Uri(it.latest);
            var version = uri.LocalPath.Trim('/').Split('/').Skip(3).FirstOrDefault();
            var type = System.IO.Path.GetExtension(it.latest).TrimStart('.');
            yield return new OnlinePackageItemViewModel
            {
                Id = it.latest.ToHashGuid(),
                Name = GetFileName(it.name, version, type),
                PackageName = it.name,
                FullUrl = it.latest,
                Description = infoMaps[it.name],
                Type = type,
                Version = version
            };
        }
    }

    private class BootCDNResult
    {
        public string name { get; set; }
        public string latest { get; set; }
    }

    private class BootCDNResponse
    {
        public List<BootCDNResult> results { get; set; }
    }
}
