using System.IO;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules
{



    public class PublicModuleFiles : IApi
    {
        public bool RequireSite => true;

        public bool RequireUser => false;

        public string ModelName => "PublicModuleFiles";


        #region General 

        internal ModuleContext GetModuleContext(ApiCall call)
        {
            call.Context.EnableCORS();
            return ScriptModuleHelper.GetModuleContextById(call);
        }

        internal virtual DiskHandler GetDiskHanlder(ApiCall call, string objectType)
        {
            call.Context.EnableCORS();
            return ScriptModuleHelper.GetResDiskHanlder(call, objectType);
        }

        #endregion

        #region Folders

        // SubFolders, use / or \ for root. 
        public List<ModuleFolderInfo> SubFolders(string folder, string objectType, ApiCall call)
        {
            call.Context.EnableCORS();
            var diskHandler = this.GetDiskHanlder(call, objectType);
            return diskHandler.SubFolders(folder);
        }

        public void CreateFolder(string folder, string parent, string objectType, ApiCall call)
        {
            call.Context.EnableCORS();
            var diskHandler = this.GetDiskHanlder(call, objectType);
            diskHandler.CreateFolder(folder, parent);
        }

        public void DeleteFolder(string Folder, string objectType, ApiCall call)
        {
            call.Context.EnableCORS();
            var diskHandler = this.GetDiskHanlder(call, objectType);
            diskHandler.DeleteFolder(Folder);
        }

        #endregion

        #region ReadWrite

        public string GetText(string Folder, string FileName, string objectType, ApiCall call)
        {
            call.Context.EnableCORS();
            var diskHandler = this.GetDiskHanlder(call, objectType);
            return diskHandler.Read(Folder, FileName);
        }

        public void UpdateText(string folder, string fileName, string content, string objectType, ApiCall call)
        {
            call.Context.EnableCORS();
            // ensure extension.
            if (!fileName.Contains("."))
            {
                var type = new ResourceType(objectType);
                if (type != null && !string.IsNullOrEmpty(type.defaultExtension))
                {
                    fileName = fileName + type.defaultExtension;
                }
            }

            var diskHandler = this.GetDiskHanlder(call, objectType);
            diskHandler.write(folder, fileName, content);
        }

        public void UpdateBinary(string folder, string fileName, string Base64, string objectType, ApiCall call)
        {
            call.Context.EnableCORS();
            var diskHandler = this.GetDiskHanlder(call, objectType);

            var binary = Convert.FromBase64String(Base64);

            diskHandler.writeBinary(folder, fileName, binary);
        }


        public void Remove(string folder, string fileName, string objectType, ApiCall call)
        {
            call.Context.EnableCORS();
            var diskHandler = this.GetDiskHanlder(call, objectType);
            diskHandler.delete(folder, fileName);
        }

        #endregion 

        #region FileList 

        public List<ModuleFileViewModel> AllFiles(ApiCall call)
        {
            call.Context.EnableCORS();
            List<ModuleFileViewModel> result = new List<ModuleFileViewModel>();

            var context = this.GetModuleContext(call);

            foreach (var item in ResourceTypeManager.AvailableResource())
            {
                if (item.Type != EnumResourceType.root && item.Type != EnumResourceType.undefined)
                {
                    var diskHandler = this.GetDiskHanlder(call, item.Name);
                    var files = diskHandler.GetAllFiles();
                    var list = ScriptModuleHelper.ToFileViewModel(context, files, item.Name);
                    result.AddRange(list);
                }
            }

            // Attached two default...
            //result.Add(ScriptModuleHelper.ReadMe(context));
            //result.Add(ScriptModuleHelper.ModuleConfig(context));
            result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.Readme));
            result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.Config));
            result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.DashBoard));
            result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.TaskEvent));


            return result;
        }

        //Return all files in the provided folder, return an Array of FileInfo 
        public ModuleFileViewModel[] FilesByFolder(string folder, string objectType, ApiCall call)
        {
            call.Context.EnableCORS();
            var context = this.GetModuleContext(call);
            var diskHandler = this.GetDiskHanlder(call, objectType);
            var files = diskHandler.FolderFiles(folder);
            return ScriptModuleHelper.ToFileViewModel(context, files, objectType);
        }

        #endregion


        public BinaryResponse PrivateShare(Guid ModuleId, ApiCall call)
        {
            call.Context.EnableCORS();
            var sitedb = call.WebSite.SiteDb();

            bool CanShare = sitedb.WebSite.EnablePublicModule;

            // TEMP
            CanShare = true;

            if (CanShare)
            {
                var module = sitedb.ScriptModule.Get(ModuleId);

                if (module != null && module.Online)
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

            }

            return null;

        }

        public List<ResourceType> ResourceType(ApiCall call)
        {
            call.Context.EnableCORS();
            return ResourceTypeManager.AvailableResource();
        }
    }

}
