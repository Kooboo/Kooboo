using Kooboo.Api;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules
{
    public abstract class TextResourceApi : IApi
    {
        public abstract EnumResourceType ResourceType { get; }

        #region General
        public abstract string ModelName { get; }

        public virtual string objectType
        {
            get
            {
                return this.ResourceType.ToString();
            }
        }

        public bool RequireSite => true;

        public bool RequireUser => true;

        internal ModuleContext GetModuleContext(ApiCall call)
        {
            return ScriptModuleHelper.GetModuleContextById(call);
        }

        internal virtual DiskHandler GetDiskHanlder(ApiCall call)
        {
            return ScriptModuleHelper.GetResDiskHanlder(call, this.objectType);
        }

        #endregion

        #region Folders

        // SubFolders, use / or \ for root. 
        public List<ModuleFolderInfo> SubFolders(string folder, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call);
            return diskHandler.SubFolders(folder);
        }

        public void CreateFolder(string folder, string parent, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call);
            diskHandler.CreateFolder(folder, parent);
        }

        public void DeleteFolder(string Folder, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call);
            diskHandler.DeleteFolder(Folder);
        }

        #endregion

        #region ReadWrite

        public string GetText(string Folder, string FileName, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call);
            return diskHandler.Read(Folder, FileName);
        }

        public void Update(string folder, string fileName, string content, ApiCall call)
        {
            // ensure extension.
            if (!fileName.Contains("."))
            {
                var type = new ResourceType(this.ResourceType);
                if (type != null && !string.IsNullOrEmpty(type.defaultExtension))
                {
                    fileName = fileName + type.defaultExtension;
                }
            }

            var diskHandler = this.GetDiskHanlder(call);
            diskHandler.write(folder, fileName, content);
        }

        public void Remove(string folder, string fileName, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call);
            diskHandler.delete(folder, fileName);
        }

        #endregion 

        #region FileList 

        public ModuleFileViewModel[] AllFiles(ApiCall call)
        {
            var context = this.GetModuleContext(call);
            var diskHandler = this.GetDiskHanlder(call);
            var files = diskHandler.GetAllFiles();
            return ScriptModuleHelper.ToFileViewModel(context, files, this.objectType);

        }

        //Return all files in the provided folder, return an Array of FileInfo 
        public ModuleFileViewModel[] FilesByFolder(string folder, ApiCall call)
        {
            var context = this.GetModuleContext(call);
            var diskHandler = this.GetDiskHanlder(call);
            var files = diskHandler.FolderFiles(folder);
            return ScriptModuleHelper.ToFileViewModel(context, files, this.objectType);
        }

        #endregion

    }


}
