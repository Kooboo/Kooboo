using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class Sync : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "Sync";

        public string Icon => "";

        public string Url => "Sync";

        public int Order => 5;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Sync", context);
        }
    }

    // new MenuItem{ Name = Hardcoded.GetValue("Sync", context),  Url = AdminUrl("Sync", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Synchronization}
}