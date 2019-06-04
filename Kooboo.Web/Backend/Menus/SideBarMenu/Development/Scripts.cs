using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Scripts : ISideBarMenu
    {
        public SideBarSection Parent =>  SideBarSection.Development;

        public string Name => "Scripts";

        public string Icon => "";

        public string Url => "Development/Scripts";

        public int Order => 5;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Scripts", Context); 
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Scripts", context), Url = AdminUrl("Development/Scripts", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Scripts },