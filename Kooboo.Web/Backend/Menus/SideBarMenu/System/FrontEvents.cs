using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;

namespace Kooboo.Web.Backend.Menus.SideBarMenu.System
{
    public class FrontEvents : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "FrontEvents";

        public string Icon => "";

        public string Url => "System/FrontEvents";

        public int Order => 12;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Front Events", Context);
        }
    }
}

