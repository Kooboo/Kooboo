using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class SiteLogs : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "SiteLogs";

        public string Icon => "";

        public string Url => "System/SiteLogs";

        public int Order => 6;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("SiteLogs", Context); 
        }
    }
}


//new MenuItem{ Name = Hardcoded.GetValue("SiteLogs",context),Url = AdminUrl("System/SiteLogs", siteDb), ActionRights = Sites.Authorization.Actions.Systems.SiteLogs },