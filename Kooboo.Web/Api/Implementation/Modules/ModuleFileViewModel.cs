using Kooboo.Mail.Extension;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules
{
    public class ModuleFileViewModel
    {
        public string objectType { get; set; }

        public bool IsText { get; set; }
        public bool IsBinary { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string FolderName { get; set; }

        public string Extension { get; set; }

        public long Size { get; set; }

        public string StringSize { get; set; }

        public System.DateTime LastModified { get; set; }

        public string FileContentType { get; set; }

        public string PreviewUrl { get; set; }

        public static ModuleFileViewModel FromFileInfo(ModuleContext context, ModuleFileInfo fileInfo, string objectType)
        {
            ModuleFileViewModel viewModel = new ModuleFileViewModel();
            viewModel.Name = fileInfo.Name;
            viewModel.FullName = fileInfo.FullName;
            viewModel.FolderName = fileInfo.FolderName;
            viewModel.Extension = fileInfo.Extension;
            viewModel.Size = fileInfo.Size;
            viewModel.StringSize = fileInfo.StringSize;
            viewModel.LastModified = fileInfo.LastModified;

            viewModel.objectType = objectType;


            if (!string.IsNullOrEmpty(objectType) && objectType != "\\" && objectType != "/")
            {

                ResourceType type = new ResourceType(objectType);
                viewModel.IsText = type.IsText;
                viewModel.IsBinary = type.IsBinary;
                viewModel.FileContentType = Kooboo.Lib.Helper.IOHelper.MimeType(fileInfo.Name);
            }


            viewModel.PreviewUrl = Kooboo.Sites.ScriptModules.Render.ModuleRenderHelper.PreviewUrl(context, fileInfo);

            return viewModel;
        }

        public static ModuleFileViewModel FromFileInfo(MailModuleContext context, ModuleFileInfo fileInfo, string objectType)
        {
            ModuleFileViewModel viewModel = new ModuleFileViewModel();
            viewModel.Name = fileInfo.Name;
            viewModel.FullName = fileInfo.FullName;
            viewModel.FolderName = fileInfo.FolderName;
            viewModel.Extension = fileInfo.Extension;
            viewModel.Size = fileInfo.Size;
            viewModel.StringSize = fileInfo.StringSize;
            viewModel.LastModified = fileInfo.LastModified;

            viewModel.objectType = objectType;


            if (!string.IsNullOrEmpty(objectType) && objectType != "\\" && objectType != "/")
            {

                MailResourceType type = new MailResourceType(objectType);
                viewModel.IsText = type.IsText;
                viewModel.IsBinary = type.IsBinary;
                viewModel.FileContentType = Kooboo.Lib.Helper.IOHelper.MimeType(fileInfo.Name);
            }
            else
            {
                viewModel.IsText = true;
                // viewModel.FileContentType = Lib.Helper.IOHelper.MimeType("fake.html"); 
            }

            // Get preview URL.

            var root = context.RootFolder;
            var filename = fileInfo.FullDiskFileName;
            filename = filename.Substring(root.Length);
            var relativeUrl = filename.Replace("\\", "/");

            var baseUrl = context.GetBaseUrl();

            viewModel.PreviewUrl = baseUrl + "&file=" + relativeUrl + "&objectType=" + objectType;

            return viewModel;
        }
    }
}
