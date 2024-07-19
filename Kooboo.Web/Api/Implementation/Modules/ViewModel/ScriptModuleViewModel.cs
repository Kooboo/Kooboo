using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Sites.Models;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules.ViewModel
{
    public class ScriptModuleViewModel
    {
        //The registered module identifier from Kooboo...
        //Module should register at Kooboo repository, when published.
        public string PackageName { get; set; }

        public string ModuleVersion { get; set; }

        public string RelativeFolder { get; set; }

        // The start view name, when it is not defined in the parameter
        public string StartView { get; set; }

        // Json Setting. 
        public string Settings { get; set; }

        public DateTime LastModified
        {
            get; set;
        }

        public string Name { get; set; }

        public string BackendViewUrl { get; set; }

        public bool Online { get; set; }


        public Guid Id { get; set; }


        public static ScriptModuleViewModel FromModule(RenderContext context, ScriptModule module)
        {
            var modulecontext = ModuleContext.CreateNewFromRenderContext(context, module);
            ScriptModuleViewModel model = new ScriptModuleViewModel
            {
                Name = module.Name,
                ModuleVersion = module.ModuleVersion,
                RelativeFolder = module.RelativeFolder,
                StartView = module.StartView,
                Settings = module.Settings,
                LastModified = module.LastModified,
                Id = module.Id,
                Online = module.Online,
                PackageName = module.PackageName,
            };

            var res = new ResourceType(EnumResourceType.backend);

            var diskHandler = ScriptModuleHelper.GetResDiskHanlder(res.Name, modulecontext);
            var files = diskHandler.GetAllFiles();

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
                        model.BackendViewUrl = Kooboo.Sites.ScriptModules.Render.ModuleRenderHelper.PreviewUrl(modulecontext, startview);
                    }
                }
            }

            // result.AddRange(list); 
            //// Attached two default...
            //result.Add(ScriptModuleHelper.ReadMe(context));
            //result.Add(ScriptModuleHelper.ModuleConfig(context));

            return model;
        }
    }
}
