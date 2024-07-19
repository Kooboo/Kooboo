using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.Api.Implementation.ThirdParty.Providers;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty;

internal class CodeSearchOnlineApi : ThirdPartyApiBase<OnlinePackageItemViewModel>
{
    public override string ModelName => "online-code";

    [Permission(Feature.CODE, Action = Data.Permission.Action.EDIT)]
    [Attributes.RequireParameters("file", "path")]
    public override bool Download(ApiCall call)
    {
        try
        {
            var item = JsonHelper.Deserialize<OnlinePackageItemViewModel>(call.GetValue("file"));
            var bytes = DownloadHelper.DownloadFile(item.FullUrl);
            var siteDb = call.WebSite.SiteDb();
            var code = siteDb.Code.GetByNameOrId(item.Name);
            if (code != null)
            {
                return false;
            }

            var encoding = EncodingDetector.GetEncoding(ref bytes);

            code = new Code
            {
                Name = item.Name,
                Body = encoding.GetString(bytes),
                ScriptType = Data.ScriptType.Module,
                CodeType = CodeType.CodeBlock,
            };

            siteDb.Code.AddOrUpdate(code);

            return true;
        }
        catch
        {
            return false;
        }
    }

    [Permission(Feature.CODE, Action = Data.Permission.Action.VIEW)]
    public override IEnumerable<OnlineProvider> Providers(ApiCall call)
    {
        yield return OnlineProviders.Cdnjs(call);
    }

    [Permission(Feature.CODE, Action = Data.Permission.Action.VIEW)]
    [Kooboo.Attributes.RequireParameters("keyword", "provider")]
    public override InfiniteListViewModel<OnlinePackageItemViewModel> SearchOnline(ApiCall call)
    {
        return base.SearchOnline(call);
    }

    public override IThirdPartyProvider<OnlinePackageItemViewModel> GetProvider(string provider)
    {
        return provider.ToLower() switch
        {
            ProviderNames.cdnjs => new CdnjsProvider("js", siteDb => siteDb.Code.All().Select(it => it.Name), NameBuilder),
            _ => null,
        };
    }

    private string NameBuilder(string name, string version, string fileType)
    {
        return StringHelper.ToValidFileName(name.Replace($".{fileType}", string.Empty).Replace(".", "_"));
    }
}
