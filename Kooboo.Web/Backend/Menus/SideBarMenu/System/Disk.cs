using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class Disk : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "Disk";

        public string Icon => "";

        public string Url => "System/Disk";

        public int Order => 8;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Disk", context);
        }
    }
}

// new MenuItem{ Name = Hardcoded.GetValue("Disk", context),Url = AdminUrl("System/Disk", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Disk },