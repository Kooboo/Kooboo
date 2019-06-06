using Kooboo.Data.Context;
using Kooboo.Web.Backend.Menus;
using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Menus.FeatureMenus
{
    public class Diagnosis : IFeatureMenu
    {
        public string Name => "Diagnosis";

        public string Icon => "icon icon-support";

        public string Url => "Sites/Diagnosis";

        public int Order => 3;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Kooboo.Data.Language.Hardcoded.GetValue("Diagnosis", Context);
        }
    }
}


//new MenuItem { Name = Hardcoded.GetValue("Media library", context), Icon="icon icon-picture", Url = AdminUrl("Contents/Images", siteDb) },
//            new MenuItem { Name = Hardcoded.GetValue("Pages", context), Icon = "icon icon-layers", Url = AdminUrl("Pages", siteDb) },
//            new MenuItem{ Name = Hardcoded.GetValue("Diagnosis", context), Icon = "icon icon-support", Url = AdminUrl("Sites/Diagnosis", siteDb) },