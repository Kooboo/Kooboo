using Kooboo.Mail.Extension;
using Kooboo.Sites.ScriptModules;

namespace Kooboo.Web.Api.Implementation.Mails.Modules.Render
{
    public static class MailModuleRenderHelper
    {

        public static ModuleFileInfo TryGetFileInfo(MailModuleContext mailContext, string fileName, bool IsView)
        {
            if (fileName == null)
            {
                return null;
            }

            fileName = fileName.Replace("/", "\\");

            var rootDisk = DiskHandler.FromMailModuleContext(mailContext, "");
            var fileinfo = rootDisk.GetFileInfo("", fileName);
            if (fileinfo == null)
            {
                foreach (var item in MailResourceTypeManager.AvailableResource())
                {
                    if (item.Type == EnumMailResourceType.root || item.Type == EnumMailResourceType.undefined)
                    {
                        continue;
                    }

                    if (item.Type == EnumMailResourceType.read || item.Type == EnumMailResourceType.compose || item.Type == EnumMailResourceType.backend)
                    {
                        if (IsView)
                        {
                            fileinfo = rootDisk.GetFileInfo(item.Name, fileName);
                            if (fileinfo != null)
                            {
                                return fileinfo;
                            }
                        }
                    }
                    else
                    {
                        if (!IsView)
                        {
                            fileinfo = rootDisk.GetFileInfo(item.Name, fileName);
                            if (fileinfo != null)
                            {
                                return fileinfo;
                            }
                        }
                    }
                }
            }


            if (fileName.StartsWith("\\"))
            {
                fileName = fileName.Substring(1);

            }

            var index = fileName.IndexOf("\\");

            if (index > -1)
            {
                fileName = fileName.Substring(index);

                return TryGetFileInfo(mailContext, fileName, IsView);
            }


            // try to take out the first /img/a.png 
            return null;
        }


        public static ModuleFileInfo TryGetFileInfo(MailModuleContext mailContext, string fileName)
        {
            if (fileName == null)
            {
                return null;
            }

            fileName = fileName.Replace("/", "\\");

            var rootDisk = DiskHandler.FromMailModuleContext(mailContext, "");
            var fileinfo = rootDisk.GetFileInfo("", fileName);
            if (fileinfo == null)
            {
                foreach (var item in MailResourceTypeManager.AvailableResource())
                {
                    if (item.Type == EnumMailResourceType.root || item.Type == EnumMailResourceType.undefined)
                    {
                        continue;
                    }

                    fileinfo = rootDisk.GetFileInfo(item.Name, fileName);
                    if (fileinfo != null)
                    {
                        return fileinfo;
                    }

                }
            }


            if (fileName.StartsWith("\\"))
            {
                fileName = fileName.Substring(1);

            }

            var index = fileName.IndexOf("\\");

            if (index > -1)
            {
                fileName = fileName.Substring(index);

                return TryGetFileInfo(mailContext, fileName);
            }


            // try to take out the first /img/a.png 
            return null;
        }
    }
}
