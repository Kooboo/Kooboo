using System.IO;
using System.IO.Compression;
using Kooboo.Api;
using Kooboo.Data.Context;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.ScriptModules;
using Kooboo.Web.Api.Implementation.Modules;
using Kooboo.Web.Api.Implementation.ThirdParty.Providers;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty;

internal class ModuleSearchOnline : ThirdPartyApiBase<OnlinePackageItemViewModel>
{
    public override string ModelName => "online-module";

    [Permission(Feature.MODULE, Action = Data.Permission.Action.VIEW)]
    [Kooboo.Attributes.RequireParameters("keyword", "provider")]
    public override InfiniteListViewModel<OnlinePackageItemViewModel> SearchOnline(ApiCall call)
    {
        call.Context.EnableCORS();
        return base.SearchOnline(call);
    }

    [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
    [Attributes.RequireParameters("file", "path")]
    public override bool Download(ApiCall call)
    {
        call.Context.EnableCORS();
        try
        {
            var item = JsonHelper.Deserialize<OnlinePackageItemViewModel>(call.GetValue("file"));
            if (item.Id == default)
            {
                return false;
            }
            var siteDb = call.WebSite.SiteDb();

            var url = $"{Data.UrlSetting.AppStore}/_api/Package/Download?PackageId={item.Id}";

            var bytes = DownloadHelper.DownloadFile(url);

            if (bytes == null)
            {
                return false;
            }

            var name = StringHelper.ToValidFileName(item.Name);
            name = ScriptModuleHelper.ToValidModuleName(name);

            var newModule = new ScriptModule()
            {
                Name = name,
                PackageName = item.PackageName
            };

            ModuleHelper.CreateModuleUrl(newModule, siteDb);

            siteDb.ScriptModule.AddOrUpdate(newModule);

            var context = ModuleContext.CreateNewFromRenderContext(call.Context, newModule);

            MemoryStream IOStream = new MemoryStream(bytes);

            using var archive = new ZipArchive(IOStream, ZipArchiveMode.Read);
            archive.ExtractToDirectory(context.RootFolder, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override IThirdPartyProvider<OnlinePackageItemViewModel> GetProvider(string provider)
    {
        return new KoobooProvider();
    }

    public override IEnumerable<OnlineProvider> Providers(ApiCall call)
    {
        yield return OnlineProviders.Kooboo(call);
    }

    public object TopPackages()
    {
        var url = "/_api/Package/TopPackages?type=module";
        url = UrlHelper.Combine(Data.UrlSetting.AppStore, url);
        return HttpHelper.Get2<object>(url);
    }
}
