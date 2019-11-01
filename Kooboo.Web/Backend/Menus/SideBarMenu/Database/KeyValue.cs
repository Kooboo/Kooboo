using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.Database
{
    public class KeyValue : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Database;

        public string Name => "Key-Value";

        public string Icon => "";

        public string Url => "Storage/KeyValue";

        public int Order => 3;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Key-Value", context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Key-Value",context), Url = AdminUrl("Storage/KeyValue", siteDb) }