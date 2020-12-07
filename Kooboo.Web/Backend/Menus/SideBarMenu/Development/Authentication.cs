using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Backend.Menus.SideBarMenu.Development
{
    public class Authentication : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Development;


        public string Name => "Authentication";

        public string Icon => "";

        public string Url => "Development/Authentication";

        public int Order => 11;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Authentication", Context);
        }
    }
}
