using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Forms : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Development;

        public string Name => "Forms";

        public string Icon => "";

        public string Url => "Development/Forms";

        public int Order => 3;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Forms", Context);
        }
    }
}


///  new MenuItem { Name = Hardcoded.GetValue("Forms", context), Url = AdminUrl("Development/Forms", siteDb),  ActionRights = Sites.Authorization.Actions.Developments.Forms },