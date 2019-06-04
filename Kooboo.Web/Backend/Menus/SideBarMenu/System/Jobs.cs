using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class Jobs : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "Jobs";

        public string Icon => "";

        public string Url => "System/Jobs";

        public int Order => 9;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Jobs", Context);
        }
    }
}

///   new MenuItem{ Name = Hardcoded.GetValue("Jobs", context), Url = AdminUrl("System/Jobs", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Jobs },