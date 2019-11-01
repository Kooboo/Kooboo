using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

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

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Urls", context);
        }
    }
}

//  new MenuItem { Name = Hardcoded.GetValue("Urls",context), Url = AdminUrl("Development/URLs", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Urls },