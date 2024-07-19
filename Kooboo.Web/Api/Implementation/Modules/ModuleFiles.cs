using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Data.Typescript;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules
{

    public class ModuleFiles : IApi
    {
        public bool RequireSite => true;

        public bool RequireUser => true;

        public string ModelName => "ModuleFiles";


        #region General 

        internal ModuleContext GetModuleContext(ApiCall call)
        {
            return ScriptModuleHelper.GetModuleContextById(call);
        }

        internal virtual DiskHandler GetDiskHanlder(ApiCall call, string objectType)
        {
            return ScriptModuleHelper.GetResDiskHanlder(call, objectType);
        }

        #endregion

        #region Folders

        // SubFolders, use / or \ for root.
        [Permission(Feature.MODULE, Action = Data.Permission.Action.VIEW)]
        public List<ModuleFolderInfo> SubFolders(string folder, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            return diskHandler.SubFolders(folder);
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public void CreateFolder(string folder, string parent, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            diskHandler.CreateFolder(folder, parent);
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public void DeleteFolder(string Folder, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            diskHandler.DeleteFolder(Folder);
        }

        #endregion

        #region ReadWrite
        [Permission(Feature.MODULE, Action = Data.Permission.Action.VIEW)]
        public string GetText(string Folder, string FileName, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            return diskHandler.Read(Folder, FileName);
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.VIEW)]
        public string GetTextByName(string Folder, string FileName, string objectType, ApiCall call)
        {
            var diskHandler = ScriptModuleHelper.GetResDiskHanlderByName(call, objectType);
            return diskHandler.Read(Folder, FileName);
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public void UpdateText(string folder, string fileName, string content, string objectType, ApiCall call)
        {
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

            if (RootFiles.IsRootFile(RootFileType.TaskEvent, fileName))
            {
                var context = GetModuleContext(call);
                ModuleHelper.UpdateTaskJs(context, content);
            }

            var moduleContext = GetModuleContext(call);
            TypescriptCache.CascadeRemove(string.Join('-', call.Context.WebSite.Id, moduleContext.Module.Id));
            //if (fileName == )
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public void UpdateBinary(string folder, string fileName, string Base64, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);

            var binary = Convert.FromBase64String(Base64);

            diskHandler.writeBinary(folder, fileName, binary);
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public void Remove(string folder, string fileName, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            var info = diskHandler.GetFileInfo(folder, fileName);
            diskHandler.delete(folder, fileName);
            var moduleContext = GetModuleContext(call);
            TypescriptCache.CascadeRemove(string.Join('-', call.Context.WebSite.Id, moduleContext.Module.Id));
        }

        #endregion 

        #region FileList 
        [Permission(Feature.MODULE, Action = Data.Permission.Action.VIEW)]
        public List<ModuleFileViewModel> AllFiles(ApiCall call)
        {
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

            // Attached   default...   
            // result.Add(ScriptModuleHelper.ReadMe(context));
            // result.Add(ScriptModuleHelper.ModuleConfig(context));
            result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.Readme));
            result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.Config));
            // result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.DashBoard));
            result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.TaskEvent));
            return result;
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.VIEW)]
        //Return all files in the provided folder, return an Array of FileInfo 
        public ModuleFileViewModel[] FilesByFolder(string folder, string objectType, ApiCall call)
        {
            var context = this.GetModuleContext(call);
            var diskHandler = this.GetDiskHanlder(call, objectType);
            var files = diskHandler.FolderFiles(folder);
            return ScriptModuleHelper.ToFileViewModel(context, files, objectType);
        }

        #endregion


    }

}
