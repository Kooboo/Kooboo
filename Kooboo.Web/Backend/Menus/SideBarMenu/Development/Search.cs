using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Search : ISideBarMenu
    {
        public SideBarSection Parent =>  SideBarSection.Development;

        public string Name => "Search";

        public string Icon => "";

        public string Url => "Development/Search";

        public int Order => 9;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Search", Context); 
        }
    }
}

// new MenuItem { Name = Hardcoded.GetValue("Search", context),  Url = AdminUrl("Development/Search", siteDb) , ActionRights = Sites.Authorization.Actions.Developments.Search},