using Kooboo.Api;
using Kooboo.Data.Context;
using Kooboo.Data.Typescript;
using Kooboo.Mail.Extension;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Models;
using Kooboo.Web.Api.Implementation.Modules;

namespace Kooboo.Web.Api.Implementation.Mails.Modules
{

    public class MailModuleFiles : IApi
    {
        public bool RequireSite => false;

        public bool RequireUser => true;

        public string ModelName => "MailModuleFiles";

        #region General 

        internal MailModuleContext GetModuleContext(ApiCall call)
        {
            var moduleId = call.GetGuidValue("moduleid");

            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(moduleId);

            var context = MailModuleContext.FromRenderContext(call.Context, module);
            context.OrgDb = orgDb;
            return context;
        }

        internal DiskHandler GetDiskHanlder(ApiCall call, string objectType)
        {
            var context = GetModuleContext(call);

            return DiskHandler.FromMailModuleContext(context, objectType);

        }

        #endregion

        #region Folders

        // SubFolders, use / or \ for root. 
        public List<ModuleFolderInfo> SubFolders(string folder, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            return diskHandler.SubFolders(folder);
        }

        public void CreateFolder(string folder, string parent, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            diskHandler.CreateFolder(folder, parent);
        }


        public void DeleteFolder(string Folder, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            diskHandler.DeleteFolder(Folder);
        }

        #endregion

        #region ReadWrite

        public string GetText(string Folder, string FileName, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            return diskHandler.Read(Folder, FileName);
        }

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
            var context = GetModuleContext(call);

            if (RootFiles.IsRootFile(RootFileType.TaskEvent, fileName))
            {
                MailModuleHelper.UpdateTaskJs(context, content);
            }

            TypescriptCache.CascadeRemove(string.Join('-', Guid.Empty, context.Module.Id));

            //if (fileName == )
        }


        public void UpdateBinary(string folder, string fileName, string Base64, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);

            var binary = Convert.FromBase64String(Base64);

            diskHandler.writeBinary(folder, fileName, binary);
        }


        public void Remove(string folder, string fileName, string objectType, ApiCall call)
        {
            var diskHandler = this.GetDiskHanlder(call, objectType);
            var context = GetModuleContext(call);
            diskHandler.delete(folder, fileName);
            TypescriptCache.CascadeRemove(string.Join('-', Guid.Empty, context.Module.Id));
        }

        #endregion

        #region FileList 

        public List<MailResourceType> ResourceType(ApiCall call)
        {
            call.Context.EnableCORS();
            return MailResourceTypeManager.AvailableResource();
        }

        public List<ModuleFileViewModel> AllFiles(ApiCall call)
        {
            List<ModuleFileViewModel> result = new List<ModuleFileViewModel>();

            var context = this.GetModuleContext(call);

            foreach (var item in MailResourceTypeManager.AvailableResource())
            {
                if (item.Type != EnumMailResourceType.root && item.Type != EnumMailResourceType.undefined)
                {
                    var diskHandler = this.GetDiskHanlder(call, item.Name);
                    var files = diskHandler.GetAllFiles();

                    var list = ScriptModuleHelper.ToFileViewModel(context, files, item.Name);
                    result.AddRange(list);
                }
            }

            List<string> RootNames = new List<string>();
            RootNames.Add("Readme.md");
            RootNames.Add("Module.config");
            RootNames.Add("Event.js");

            DiskHandler Rootdisk = this.GetDiskHanlder(call, "");

            foreach (var item in RootNames)
            {
                var rootFileName = System.IO.Path.Combine(context.RootFolder, item);
                if (System.IO.File.Exists(rootFileName))
                {
                    var infox = Rootdisk.GetFileInfo(rootFileName);

                    var model = ModuleFileViewModel.FromFileInfo(context, infox, EnumMailResourceType.root.ToString());
                    result.Add(model);
                }
            }

            //var filename = System.IO.Path.Combine(context.RootFolder, rootfilename);

            //if (!System.IO.File.Exists(filename))
            //{
            //    var initText = RootFiles.GetRootFileDefaultValue(fileType, context);
            //    System.IO.File.WriteAllText(filename, initText);
            //}
            //var info = disk.GetFileInfo(filename);



            //var rootfilename = RootFiles.GetRootFileName(fileType);

            //DiskHandler disk = DiskHandler.FromModuleContext(context, "");
            //var filename = System.IO.Path.Combine(context.RootFolder, rootfilename);

            //if (!System.IO.File.Exists(filename))
            //{
            //    var initText = RootFiles.GetRootFileDefaultValue(fileType, context);
            //    System.IO.File.WriteAllText(filename, initText);
            //}
            //var info = disk.GetFileInfo(filename);
            //return info;


            //var rootdisk = this.GetDiskHanlder(call, "");

            //var rootFiles = rootdisk.GetAllFiles();

            //var rootlist = ScriptModuleHelper.ToFileViewModel(context, rootFiles, "");
            //result.AddRange(rootlist);

            // Attached   default... 

            //TODO: to be checked..

            //result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.Readme));
            //result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.Config));
            //// result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.DashBoard));
            //result.Add(ScriptModuleHelper.ModuleRootFileViewModel(context, RootFileType.TaskEvent));
            return result;
        }

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
