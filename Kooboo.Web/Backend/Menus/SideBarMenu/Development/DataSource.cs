using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class DataSource : ISideBarMenu
    {
        public SideBarSection Parent =>  SideBarSection.Development;

        public string Name => "DataSource";

        public string Icon =>"";

        public string Url => "Development/DataSources";

        public int Order => 10;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("DataSource", Context); 
        }
    }
}

// new MenuItem { Name = Hardcoded.GetValue("DataSource",context), Url = AdminUrl("Development/DataSources", siteDb), ActionRights = Sites.Authorization.Actions.Developments.DataSource }