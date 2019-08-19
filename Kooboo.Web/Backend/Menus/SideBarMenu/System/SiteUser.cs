using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class SiteUser : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "SiteUser";

        public string Icon => "";

        public string Url => "System/SiteUser";

        public int Order => 10;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("SiteUser", Context);
        }
    }
}


// new MenuItem{ Name = Hardcoded.GetValue("SiteUser", context), Url = AdminUrl("System/SiteUser", siteDb), ActionRights = Sites.Authorization.Actions.Systems.SiteUser }