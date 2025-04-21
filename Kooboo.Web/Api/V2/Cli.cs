using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using JWT;
using JWT.Serializers;
using Kooboo.Api;
using Kooboo.Data.Config;
using Kooboo.Data.Helper;
using Kooboo.Data.Unocss;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.Service;
using Kooboo.Sites.ViewModel;
using Kooboo.Web.Api.Implementation.Modules;

namespace Kooboo.Web.Api.V2;

public class Cli : IApi
{
    public string ModelName => "cli";

    public bool RequireSite => false;

    public bool RequireUser => false;

    public record TokenInfo(string Token, long ExpireAt, string ServerUrl);
    public record SiteInfo(Guid Id, string SiteUrl);
    public record ModuleInfo(string ModuleUrl);

    public TokenInfo GenerateToken(string userName, string password, ApiCall call)
    {
        var user = Kooboo.Data.GlobalDb.Users.Validate(userName, password);
        var token = user.OneTimeToken;

        IJsonSerializer serializer = new JsonNetSerializer();
        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        IJwtDecoder decoder = new JwtDecoder(serializer, urlEncoder);
        var serverUrl = $"{call.Context.Request.Scheme}://{call.Context.Request.Host}";
        var json = decoder.Decode(token);
        var jsonRoot = JsonSerializer.Deserialize<JsonElement>(json);
        if (jsonRoot.TryGetProperty("redirect", out var jsonRedirect))
        {
            serverUrl = $"https://{jsonRedirect.GetString()}";
        }

        long expireAt = 0;
        if (jsonRoot.TryGetProperty("exp", out var jsonExp))
        {
            expireAt = jsonExp.GetInt64();
        }

        return new TokenInfo(token, expireAt, serverUrl);
    }

    public SiteInfo CreateSite(string name, ApiCall call)
    {
        name = Regex.Replace(name, @"[^\w]", "");
        if (string.IsNullOrWhiteSpace(name)) throw new Exception("Site name invalid");
        var domains = DomainService.Available(call.Context);
        var rootDomain = domains?.FirstOrDefault()?.DomainName;
        if (rootDomain == null) throw new Exception("RootDomain not found");
        var orgId = call.Context.User.CurrentOrgId;
        name = GetAvailableSiteName(orgId, name);
        var fullDomain = GetAvailableDomain(rootDomain, name);
        var newSite = WebSiteService.AddNewSite(
            orgId,
            name,
            fullDomain,
            call.Context.User.Id,
            true
        );
        Sites.Scripting.Global.Koobox.KFavorite.Add(call.Context, newSite.Id);
        return new SiteInfo(newSite.Id, newSite.BaseUrl());
    }

    public ModuleInfo InstallModule(string name, ApiCall call)
    {
        var site = AppHost.SiteRepo.Get(call.Context.User.CurrentOrgId, call.WebSite.Id);
        if (site == null) throw new Exception("WebSite not found");
        var searchUrl = $"{Data.UrlSetting.AppStore}/_api/Package/search?type=module&count=100&keyword={name}";
        var list = HttpHelper.Get<List<RemoteTemplatePackage>>(searchUrl);
        var module = list.FirstOrDefault(f => f.Name == name);
        if (module == null) throw new Exception("Module not found");
        var downloadUrl = $"{Data.UrlSetting.AppStore}/_api/Package/Download?PackageId={module.Id}";
        var bytes = DownloadHelper.DownloadFile(downloadUrl);
        name = StringHelper.ToValidFileName(module.Name);
        name = ScriptModuleHelper.ToValidModuleName(name);

        var newModule = new ScriptModule()
        {
            Name = name,
            PackageName = module.Name
        };

        var siteDb = site.SiteDb();
        var moduleRoute = ModuleHelper.CreateModuleUrl(newModule, siteDb); ///_sqlite_orm/{part}
        siteDb.ScriptModule.AddOrUpdate(newModule);
        var context = ModuleContext.CreateNewFromRenderContext(call.Context, newModule);
        MemoryStream IOStream = new MemoryStream(bytes);
        using var archive = new ZipArchive(IOStream, ZipArchiveMode.Read);
        archive.ExtractToDirectory(context.RootFolder, true);
        moduleRoute = moduleRoute[..moduleRoute.LastIndexOf('/')];
        var moduleUrl = $"{call.Context.Request.Scheme}://{call.Context.Request.Host}{moduleRoute}";
        return new ModuleInfo(moduleUrl);
    }

    public void UpdateUnocss(ApiCall call)
    {
        var site = AppHost.SiteRepo.Get(call.Context.User.CurrentOrgId, call.WebSite.Id);
        if (site == null) throw new Exception("WebSite not found");
        var settings = JsonSerializer.Deserialize<UnocssSettings>(call.Context.Request.Body);
        site.UnocssSettings = settings;
        AppHost.SiteRepo.AddOrUpdate(site);
    }

    private static string GetAvailableDomain(string rootDomain, string name)
    {
        for (int i = 0; i < 99; i++)
        {
            var subDomain = i > 0 ? name + i : name;
            var fullDomain = ConfigHelper.ToFullDomain(rootDomain, subDomain);
            var binding = AppHost.BindingService.GetByFullDomain(fullDomain);
            if (binding == null || binding.Count == 0)
            {
                return fullDomain;
            }
        }

        throw new Exception("Not found available domain");
    }

    private static string GetAvailableSiteName(Guid orgId, string name)
    {
        for (int i = 0; i < 99; i++)
        {
            var siteName = i > 0 ? name + i : name;
            var available = AppHost.SiteService.CheckNameAvailable(siteName, orgId);
            if (available)
            {
                return siteName;
            }
        }

        throw new Exception("Not found available site name");
    }
}