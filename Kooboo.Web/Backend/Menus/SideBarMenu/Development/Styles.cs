using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Styles : ISideBarMenu
    {
        public SideBarSection Parent =>  SideBarSection.Development;

        public string Name => "Styles";

        public string Icon => "";

        public string Url => "Development/Styles";

        public int Order => 6;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
           return Hardcoded.GetValue("Styles", Context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Styles", context), Url = AdminUrl("Development/Styles", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Styles }