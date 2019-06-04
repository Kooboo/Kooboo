using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Views : ISideBarMenu
    {
        public SideBarSection Parent =>  SideBarSection.Development;

        public string Name => "Views";

        public string Icon =>"";

        public string Url => "Development/Views";

        public int Order => 2;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Views", Context);
        }
    }
}


//  new MenuItem { Name = Hardcoded.GetValue("Views", context), Url = AdminUrl("Development/Views", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Views },