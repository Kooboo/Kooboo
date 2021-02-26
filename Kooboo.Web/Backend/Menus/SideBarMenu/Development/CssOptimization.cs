using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class CssOptimization : ISideBarMenu
    {
        public SideBarSection Parent =>  SideBarSection.Development;

        public string Name => "CSS optimization";

        public string Icon => "";

        public string Url => "Development/CssOptimization";

        public int Order => 12;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
           return Hardcoded.GetValue("CSS optimization", Context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Styles", context), Url = AdminUrl("Development/Styles", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Styles }