using Kooboo.Api;
using Kooboo.Sites.ScriptModules;


namespace Kooboo.Web.Api.Implementation.Modules
{

    public abstract class BinaryResourceApi : IApi
    {
        public abstract string ModelName { get; }

        public abstract string objectType { get; }

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


        public void Update(string folder, string fileName, string Base64, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call);

            var binary = Convert.FromBase64String(Base64);

            diskHandler.writeBinary(folder, fileName, binary);
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
