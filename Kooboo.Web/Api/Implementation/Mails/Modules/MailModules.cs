using System.IO;
using System.IO.Compression;
using System.Linq;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Typescript;
using Kooboo.Mail.Extension;
using Kooboo.Mail.Models;
using Kooboo.Sites.Scripting.Global.Database;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Models;
using Kooboo.Sites.ViewModel;
using Kooboo.Web.Api.Implementation.Modules;

namespace Kooboo.Web.Api.Implementation.Mails.Modules
{

    public class MailModules : IApi
    {
        public string ModelName => "MailModule";
        public bool RequireSite => false;
        public bool RequireUser => true;

        public List<MailModuleViewModel> List(ApiCall call)
        {
            call.Context.EnableCORS();
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);
            var mailModules = orgDb.Module.All();
            var result = new List<MailModuleViewModel>();
            foreach (var mailModule in mailModules)
            {
                var mailModuleViewModel = new MailModuleViewModel
                {
                    Id = mailModule.Id,
                    Name = mailModule.Name,
                    Settings = mailModule.Settings,
                    TaskJs = mailModule.TaskJs,
                    Online = mailModule.Online
                };

                var module = orgDb.Module.Get(mailModule.Id);
                var modulecontext = MailModuleContext.CreateNewFromRenderContext(call.Context, module);
                var res = new ResourceType(EnumResourceType.backend);
                var handler = DiskHandler.FromMailModuleContext(modulecontext, "backend");
                var files = handler.GetAllFiles();

                //TODO: tmep, make the file extension defined in a setting. 

                if (files.Any())
                {
                    var filelist = files.ToList();


                    var backendviews = filelist.FindAll(o => o.Extension != null && o.Extension.EndsWith(".html"));

                    if (backendviews != null && backendviews.Any())
                    {
                        var startview = backendviews.Find(o => ScriptModuleHelper.IsDefaultStartName(o.Name));
                        if (startview == null)
                        {
                            startview = backendviews.FirstOrDefault();
                        }


                        if (startview != null)
                        {

                            mailModuleViewModel.BackendViewUrl = $"/_api/MailExtension/Render?moduleId={module.Id}&file=/backend/{startview.FullName}&objectType=backend";
                        }
                    }

                }

                result.Add(mailModuleViewModel);
            }

            return result;

        }

        // mark it online or offline. 
        public void UpdateStatus(Guid ModuleId, bool Status, ApiCall call)
        {
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(ModuleId);

            if (module != null)
            {
                module.Online = Status;
                orgDb.Module.AddOrUpdate(module);
            }
        }


        public MailModule Create(string Name, ApiCall call)
        {
            call.Context.EnableCORS();

            Name = Kooboo.Lib.Helper.StringHelper.ToValidFileName(Name);

            MailModule module = new MailModule();
            module.Name = Name;

            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var exists = orgDb.Module.Get(Name);
            if (exists != null)
            {
                throw new Exception("Name already exists");
            }

            orgDb.Module.AddOrUpdate(module);

            // init  files...
            var moduleContext = MailModuleContext.FromRenderContext(call.Context, module);

            InitModuleFiles(moduleContext, module.Name);

            return module;

        }

        private void InitModuleFiles(MailModuleContext context, string moduleName)
        {
            // case RootFileType.Readme:
            // return "Readme.md";
            // case RootFileType.Config:
            // return "Module.config"; 
            // case RootFileType.TaskEvent:
            // return "Event.js";

            List<string> RootNames = new List<string>();
            RootNames.Add("Readme.md");
            RootNames.Add("Module.config");
            RootNames.Add("Event.js");

            DiskHandler rootdisk = DiskHandler.FromMailModuleContext(context, "");  // root. 
            foreach (var item in RootNames)
            {
                var defaultValue = GetDefaultValue(item, context.Module.Name);
                rootdisk.write("", item, defaultValue);
            }

            return;
        }

        private string GetDefaultValue(string filename, string moduleName)
        {
            if (filename == "Module.config")
            {
                string defaultvalue = @"{
  ""name"": ""{my_module}"",
  ""description"": ""{my_desc}"",
  ""version"": ""1.0.0"",
  ""settingDefines"": [
    {
      ""name"": ""text"",
      ""defaultValue"": ""Dummy text"",
      ""description"": ""text description"",
      ""display"": ""Text Input"",
      ""type"": ""input""
    },
    {
      ""name"": ""number"",
      ""defaultValue"": 1234,
      ""description"": ""number description"",
      ""display"": ""number"",
      ""type"": ""number""
    },
    {
      ""name"": ""selection"",
      ""defaultValue"": ""option1"",
      ""description"": ""selecttion description"",
      ""display"": ""Selection"",
      ""type"": ""select"",
      ""options"": [
        ""option1"",
        ""option2""
      ]
    },
    {
      ""name"": ""switch"",
      ""defaultValue"": true,
      ""description"": ""switch description"",
      ""display"": ""boolean"",
      ""type"": ""switch""
    },
    {
      ""name"": ""textarea"",
      ""defaultValue"": ""textareaDefault"",
      ""description"": ""textarea description"",
      ""type"": ""textarea""
    }
  ]
}";
                var result = defaultvalue.Replace("{my_module}", moduleName);
                result = result.Replace("{my_desc}", "Module Summary Information");
                return result;

            }
            else if (filename == "Readme.md")
            {
                return "<p>Your module information in **MarkDown** syntax</p> <p>Module config value can be access from k.Mail.Module.Config</p>";
            }
            else if (filename == "Event.js")
            {
                return "// your event code."; // k.Mail.Event.OnMessageReceived(function(msg) {});";
            }
            else if (filename == "backend.html")
            {
                return "// your view for backend management page";
            }
            else if (filename == "reply.html")
            {
                return "// your view for backend management page";
            }
            else if (filename == "read.html")
            {
                return "// your view for backend management page";
            }

            return null;

        }


        public void Delete(Guid Id, ApiCall call)
        {
            call.Context.EnableCORS();

            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(Id);

            if (module != null)
            {
                var context = MailModuleContext.FromRenderContext(call.Context, module);
                KModuleDatabase db = new KModuleDatabase(call.Context, context.RootFolder);
                db.Close();

                orgDb.Module.Delete(Id);
                TypescriptCache.CascadeRemove(string.Join('-', Guid.Empty, Id));

                try
                {
                    System.IO.Directory.Delete(context.RootFolder, true);

                }
                catch (Exception)
                {

                }
            }

        }


        public void Deletes(Guid[] Ids, ApiCall call)
        {
            call.Context.EnableCORS();

            foreach (var item in Ids)
            {
                Delete(item, call);
            }
        }


        public List<MailResourceType> ResourceType(ApiCall call)
        {
            call.Context.EnableCORS();
            return Kooboo.Mail.Extension.MailResourceTypeManager.AvailableResource();
        }

        public Guid Import(ApiCall call)
        {
            call.Context.EnableCORS();

            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var files = call.Context.Request.Files;

            if (files == null || files.Count() == 0)
            {
                return default(Guid);
            }

            string modulename;

            if (call.Context.Request.Forms != null)
            {
                modulename = call.Context.Request.Forms["name"];

                var newModule = new MailModule() { Name = modulename };

                orgDb.Module.AddOrUpdate(newModule);

                var context = MailModuleContext.FromRenderContext(call.Context, newModule);

                MemoryStream IOStream = new MemoryStream(files[0].Bytes);

                using (var archive = new ZipArchive(IOStream, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(context.RootFolder);
                }
                return newModule.Id;
            }

            return default(Guid);
        }

        public BinaryResponse Export(string Name, ApiCall call)
        {
            call.Context.EnableCORS();

            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(Name);

            if (module != null)
            {
                var strGuid = System.Guid.NewGuid().ToString();

                var context = MailModuleContext.CreateNewFromRenderContext(call.Context, module);
                KModuleDatabase db = new KModuleDatabase(call.Context, context.RootFolder);
                db.Close();

                var zipFile = System.IO.Path.Combine(AppSettings.TempDataPath, strGuid + ".zip");
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }

                ZipFile.CreateFromDirectory(context.RootFolder, zipFile);

                var allbytes = File.ReadAllBytes(zipFile);

                BinaryResponse response = new BinaryResponse();
                response.ContentType = "application/zip";
                response.Headers.Add("Content-Disposition", $"attachment;filename=module.zip");
                response.BinaryBytes = allbytes;
                return response;
            }

            return null;
        }



        public void Share(ApiCall call)
        {
            call.Context.EnableCORS();

            var url = Data.UrlSetting.AppStore + "/_api/Package/ModuleShare?type=email&UserId=" + call.Context.User.Id.ToString();


            var id = call.GetValue<Guid>("id");

            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(id);

            if (module != null)
            {
                var context = MailModuleContext.CreateNewFromRenderContext(call.Context, module);
                KModuleDatabase db = new KModuleDatabase(call.Context, context.RootFolder);
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

                ZipFile.CreateFromDirectory(context.RootFolder, zipFile);
                var allbytes = File.ReadAllBytes(zipFile);

                var base64 = System.Text.Encoding.UTF8.GetBytes(Convert.ToBase64String(allbytes));

                Kooboo.Lib.Helper.HttpHelper.PostBinary(url, null, allbytes);
            }
        }

        public List<ModuleSearchResult> Search(string keyword, ApiCall call)
        {
            call.Context.EnableCORS();
            var url = UrlSetting.AppStore + "/_api/Package/search?type=email&count=100&keyword=" + keyword;

            var list = Lib.Helper.HttpHelper.Get<List<RemoteTemplatePackage>>(url);

            List<ModuleSearchResult> result = new List<ModuleSearchResult>();

            foreach (var item in list)
            {
                ModuleSearchResult model = new ModuleSearchResult();

                model.Title = item.Title == null ? item.Name : item.Title;

                model.Description = item.Description;

                model.PackageId = item.Id.ToString();

                result.Add(model);
            }

            return result;
        }


        public Guid InstallModule(string PackageId, string Name, ApiCall call)
        {
            call.Context.EnableCORS();
            var url = Data.UrlSetting.AppStore + "/_api/Package/Download?PackageId=" + PackageId;

            var bytes = Lib.Helper.DownloadHelper.DownloadFile(url);

            if (bytes != null)
            {
                var newModule = new MailModule() { Name = Name };

                var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

                orgDb.Module.AddOrUpdate(newModule);

                var context = MailModuleContext.CreateNewFromRenderContext(call.Context, newModule);

                MemoryStream IOStream = new MemoryStream(bytes);

                using (var archive = new ZipArchive(IOStream, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(context.RootFolder, true);
                }
                return newModule.Id;
            }

            return default(Guid);
        }
    }

}
