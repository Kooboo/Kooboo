using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Web.Backend.Menus;
using Kooboo.Web.Menus;

namespace Kooboo.Web.Menus.FeatureMenus
{
    public class MediaMenu : IFeatureMenu
    {
        public string Name => "Media";

        public string Icon => "icon icon-picture";

        public string Url => "Contents/Images";
         
        public int Order => 1;

        public List<ICmsMenu> SubItems { get; set; }

       //public bool UrlSiteId => true;

        public string GetDisplayName(RenderContext Context)
        {
            return Kooboo.Data.Language.Hardcoded.GetValue("Media library", Context);
        }
    }



    //new MenuItem { Name = Hardcoded.GetValue("Media library", context), Icon="icon icon-picture", Url = AdminUrl("Contents/Images", siteDb) },
    //            new MenuItem { Name = Hardcoded.GetValue("Pages", context), Icon = "icon icon-layers", Url = AdminUrl("Pages", siteDb) },
    //            new MenuItem{ Name = Hardcoded.GetValue("Diagnosis", context), Icon = "icon icon-support", Url = AdminUrl("Sites/Diagnosis", siteDb) },


}
