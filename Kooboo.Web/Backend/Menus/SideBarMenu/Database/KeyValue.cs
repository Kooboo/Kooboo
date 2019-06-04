using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Database
{
    public class KeyValue : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Database;

        public string Name => "Key-Value";

        public string Icon => "";

        public string Url => "Storage/KeyValue";

        public int Order =>3;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Key-Value", Context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Key-Value",context), Url = AdminUrl("Storage/KeyValue", siteDb) }