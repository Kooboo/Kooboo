using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class Domains : ISideBarMenu
    {
        public SideBarSection Parent =>  SideBarSection.System;

        public string Name => "Domains";

        public string Icon => "";

        public string Url => "System/Domains";

        public int Order => 4;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Domains", Context); 
        }
    } 

    // new MenuItem{ Name = Hardcoded.GetValue("Domains", context), Url = AdminUrl("System/Domains", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Domains
} 
 
