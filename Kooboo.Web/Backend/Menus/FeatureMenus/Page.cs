using Kooboo.Data.Context;
using Kooboo.Web.Backend.Menus;
using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Menus.FeatureMenus
{
    public class Page : IFeatureMenu
    {
        public string Name => "Pages";

        public string Icon => "icon icon-layers";

        public string Url => "Pages";

        public int Order => 2;

        public List<ICmsMenu> SubItems { get; set; }
         
        public string GetDisplayName(RenderContext Context)
        {
            return Kooboo.Data.Language.Hardcoded.GetValue("Pages", Context);
        }
    }
}


//new MenuItem { Name = Hardcoded.GetValue("Media library", context), Icon="icon icon-picture", Url = AdminUrl("Contents/Images", siteDb) },
//            new MenuItem { Name = Hardcoded.GetValue("Pages", context), Icon = "icon icon-layers", Url = AdminUrl("Pages", siteDb) },
//            new MenuItem{ Name = Hardcoded.GetValue("Diagnosis", context), Icon = "icon icon-support", Url = AdminUrl("Sites/Diagnosis", siteDb) },