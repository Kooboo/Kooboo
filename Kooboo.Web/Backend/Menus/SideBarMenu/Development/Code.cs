using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Code : ISideBarMenu
    {
        public SideBarSection Parent =>  SideBarSection.Development;

        public string Name => "Code";

        public string Icon => "";

        public string Url => "Development/Code";

        public int Order => 7;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Code", Context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Code",context), Url = AdminUrl("Development/Code", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Code }