using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;

namespace Kooboo.Web.Backend.Menus.SideBarMenu.System
{
    public class CoreSettings : ISideBarMenu
    {

        public SideBarSection Parent => SideBarSection.System;

        public string Name => "Config";

        public string Icon => "";

        public string Url => "System/CoreSettings";

        public int Order => 2;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Config", Context);
        }

    }


    //new MenuItem{ Name = Hardcoded.GetValue("Config",context), Url = AdminUrl("System/CoreSettings", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Settings },



}
