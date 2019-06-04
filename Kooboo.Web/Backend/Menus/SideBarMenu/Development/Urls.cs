using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Urls : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Development;

        public string Name => "Urls";

        public string Icon => "";

        public string Url => "Development/URLs";

        public int Order => 8;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Urls", Context);
        }
    }
}


//  new MenuItem { Name = Hardcoded.GetValue("Urls",context), Url = AdminUrl("Development/URLs", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Urls },