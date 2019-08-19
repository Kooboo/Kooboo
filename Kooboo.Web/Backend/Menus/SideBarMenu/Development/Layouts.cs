using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Layouts : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Development;

        public string Name => "Layouts";

        public string Icon => "";

        public string Url => "Development/Layouts";

        public int Order => 1;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Layouts", Context);
        }
    }
}


// new MenuItem { Name = Hardcoded.GetValue("Layouts", context), Url = AdminUrl("Development/Layouts", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Layouts },