using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Menus : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Development;

        public string Name => "Menus";

        public string Icon => "";

        public string Url => "Development/Menus";

        public int Order => 4;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Menus", context);
        }
    }
}

//   new MenuItem { Name = Hardcoded.GetValue("Menus", context),  Url = AdminUrl("Development/Menus", siteDb),  ActionRights = Sites.Authorization.Actions.Developments.Menus }