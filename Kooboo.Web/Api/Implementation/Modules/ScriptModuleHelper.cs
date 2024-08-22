using System.Linq;
using Kooboo.Api;
using Kooboo.Mail.Extension;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.ScriptModules;

namespace Kooboo.Web.Api.Implementation.Modules
{
    public static class ScriptModuleHelper
    {
        // used for backend, when moduleid is pass as a parameter. 
        public static ModuleContext GetModuleContextById(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            ScriptModule module = null;

            var id = call.GetGuidValue("moduleid");
            if (id != default(Guid))
            {
                module = sitedb.ScriptModule.Get(id);

                return Sites.ScriptModules.ModuleContext.FromRenderContext(call.Context, module);
            }
            return null;
        }

        public static ModuleContext GetModuleContextByName(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            ScriptModule module = null;

            var name = call.GetValue("moduleName");
            if (name != default)
            {
                module = sitedb.ScriptModule.GetByNameOrId(name);

                return Sites.ScriptModules.ModuleContext.FromRenderContext(call.Context, module);
            }
            return null;
        }


        public static DiskHandler GetResDiskHanlder(ApiCall call, string objectType)
        {
            var context = GetModuleContextById(call);
            return GetResDiskHanlder(objectType, context);
        }

        public static DiskHandler GetResDiskHanlderByName(ApiCall call, string objectType)
        {
            var context = GetModuleContextByName(call);
            return GetResDiskHanlder(objectType, context);
        }

        public static DiskHandler GetResDiskHanlder(string objectType, ModuleContext context)
        {
            return DiskHandler.FromModuleContext(context, objectType);
        }

        public static ModuleFileViewModel[] ToFileViewModel(ModuleContext context, ModuleFileInfo[] Files, string objectType)
        {
            return Files.Select(o => ModuleFileViewModel.FromFileInfo(context, o, objectType)).ToArray();
        }

        public static ModuleFileViewModel[] ToFileViewModel(MailModuleContext context, ModuleFileInfo[] Files, string objectType)
        {
            return Files.Select(o => ModuleFileViewModel.FromFileInfo(context, o, objectType)).ToArray();
        }

        public static ModuleFileViewModel ModuleRootFileViewModel(ModuleContext context, RootFileType fileType)
        {
            ModuleFileInfo info = ModuleContextHelper.ModuleRootFileInfo(context, fileType);
            var result = ModuleFileViewModel.FromFileInfo(context, info, "/");
            result.IsText = true;
            result.objectType = "root";
            return result;
        }

        public static bool IsDefaultStartName(string input)
        {
            if (input == null)
            {
                return false;
            }

            input = input.ToLower();

            if (input.StartsWith("index") || input.StartsWith("default") || input.StartsWith("start") || input.StartsWith("home"))
            {
                return true;
            }
            return false;
        }

        private static string[] reservedNames = ["admin", "api"];

        public static string ToValidModuleName(string name)
        {
            if (reservedNames.Any(a => a.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                name = '_' + name;
            }

            return name;
        }
    }
}
