using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Permission;
using Kooboo.Data.Typescript;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Scripting.Global.Database;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Models;
using Kooboo.Web.Api.Implementation.Modules.ViewModel;

namespace Kooboo.Web.Api.Implementation.Modules
{
    public class ScriptModuleApi : IApi
    {
        public string ModelName => "ScriptModule";
        public bool RequireSite => true;
        public bool RequireUser => true;

        [Permission(Feature.MODULE, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<ScriptModuleViewModel> List(ApiCall call)
        {
            var rendercontext = call.Context;

            call.Context.EnableCORS();
            var sitedb = call.WebSite.SiteDb();
            var all = sitedb.ScriptModule.All();
            return all.SortByNameOrCreationDate(call).Select(o => ScriptModuleViewModel.FromModule(rendercontext, o));
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public void UpdateStatus(ApiCall call, Guid ModuleId, bool Status)
        {
            var sitedb = call.WebSite.SiteDb();
            var module = sitedb.ScriptModule.Get(ModuleId);
            if (module != null)
            {
                module.Online = Status;
                sitedb.ScriptModule.AddOrUpdate(module);
            }
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public void CreateModuleUrl(ScriptModule module, ApiCall call)
        {
            call.Context.EnableCORS();
            var sitedb = call.WebSite.SiteDb();

            Kooboo.Sites.ScriptModules.ModuleHelper.CreateModuleUrl(module, sitedb);
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public ScriptModule Create(string Name, ApiCall call)
        {
            call.Context.EnableCORS();
            var sitedb = call.WebSite.SiteDb();

            Name = Kooboo.Lib.Helper.StringHelper.ToValidFileName(Name);
            Name = ScriptModuleHelper.ToValidModuleName(Name);

            ScriptModule module = new ScriptModule();
            module.Name = Name;

            CreateModuleUrl(module, call);

            sitedb.ScriptModule.AddOrUpdate(module, call.Context.User.Id);
            // init  files...
            var moduleContext = ModuleContext.FromRenderContext(call.Context, module);
            ModuleContextHelper.ModuleRootFileInfo(moduleContext, RootFileType.Config);
            ModuleContextHelper.ModuleRootFileInfo(moduleContext, RootFileType.TaskEvent);
            ModuleContextHelper.ModuleRootFileInfo(moduleContext, RootFileType.DashBoard);
            ModuleContextHelper.ModuleRootFileInfo(moduleContext, RootFileType.Readme);
            return module;

        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.DELETE)]
        public void Delete(Guid Id, ApiCall call)
        {
            call.Context.EnableCORS();
            var sitedb = call.WebSite.SiteDb();
            sitedb.ScriptModule.Delete(Id);
            TypescriptCache.CascadeRemove(string.Join('-', call.Context.WebSite.Id, Id));
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.DELETE)]
        public void Deletes(Guid[] Ids, ApiCall call)
        {
            call.Context.EnableCORS();
            foreach (var item in Ids)
            {
                Delete(item, call);
            }
        }

        public List<ResourceType> ResourceType(ApiCall call)
        {
            call.Context.EnableCORS();
            return Kooboo.Sites.ScriptModules.ResourceTypeManager.AvailableResource();
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public BinaryResponse Export(string Name, ApiCall call)
        {
            call.Context.EnableCORS();
            var sitedb = call.WebSite.SiteDb();

            var module = sitedb.ScriptModule.Get(Name);

            if (module != null)
            {
                var strGuid = System.Guid.NewGuid().ToString();

                var zipFile = System.IO.Path.Combine(AppSettings.TempDataPath, strGuid + ".zip");
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }

                ModuleContext context = ModuleContext.FromRenderContext(call.Context, module);

                System.IO.Compression.ZipFile.CreateFromDirectory(context.RootFolder, zipFile);

                var allbytes = System.IO.File.ReadAllBytes(zipFile);

                BinaryResponse response = new BinaryResponse();
                response.ContentType = "application/zip";
                response.Headers.Add("Content-Disposition", $"attachment;filename=module.zip");
                response.BinaryBytes = allbytes;
                return response;

            }

            return null;
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public Guid Import(ApiCall call)
        {
            call.Context.EnableCORS();
            var siteDb = call.WebSite.SiteDb();

            var files = call.Context.Request.Files;

            if (files == null || files.Count() == 0)
            {
                return default(Guid);
            }

            string modulename;

            if (call.Context.Request.Forms != null)
            {
                modulename = call.Context.Request.Forms["name"];
                modulename = ScriptModuleHelper.ToValidModuleName(modulename);

                var newModule = new ScriptModule() { Name = modulename };

                CreateModuleUrl(newModule, call);

                var module = siteDb.ScriptModule.AddOrUpdate(newModule);
                var context = ModuleContext.FromRenderContext(call.Context, newModule);

                MemoryStream IOStream = new MemoryStream(files[0].Bytes);

                using (var archive = new ZipArchive(IOStream, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(context.RootFolder);
                }
                return newModule.Id;
            }

            return default(Guid);
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public void Share(ApiCall call)
        {
            call.Context.EnableCORS();


            var url = Data.UrlSetting.AppStore + "/_api/Package/ModuleShare?type=module&UserId=" + call.Context.User.Id.ToString();

            var sitedb = call.WebSite.SiteDb();

            var id = call.GetValue<Guid>("id");

            var module = sitedb.ScriptModule.Get(id);

            if (module != null)
            {

                var moduleContext = ModuleContext.CreateNewFromRenderContext(call.Context, module);
                KModuleDatabase db = new KModuleDatabase(moduleContext);
                db.Close();

                var version = call.GetValue("version");
                if (version != null)
                {
                    url += "&version=" + version;
                }

                var strGuid = System.Guid.NewGuid().ToString();

                var zipFile = System.IO.Path.Combine(AppSettings.TempDataPath, strGuid + ".zip");
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }

                ModuleContext context = ModuleContext.FromRenderContext(call.Context, module);

                ZipFile.CreateFromDirectory(context.RootFolder, zipFile);
                using var fileStream = new FileStream(zipFile, FileMode.Open);

                var content = new MultipartFormDataContent
                {
                    { new StreamContent(fileStream), "module", "module.zip" }
                };

                var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = content,
                    RequestUri = new Uri(url)
                };

                foreach (var item in Data.Helper.ApiHelper.GetAuthHeaders(call.Context))
                {
                    message.Headers.Add(item.Key, item.Value);
                }

                var response = HttpClientHelper.Client.Send(message);
                using var stream = response.Content.ReadAsStream();
                using var streamReader = new StreamReader(stream);
                streamReader.ReadToEnd();
            }
        }
    }
}
