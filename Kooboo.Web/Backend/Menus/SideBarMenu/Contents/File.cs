using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;
using System.Collections.Generic;

namespace Kooboo.Web.Backend.Menus.SideBarMenu.Contents
{
    public class File : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Contents; 

        public string Name => "Files";

        public string Icon => "";

        public string Url => "Storage/Files";

        public int Order => 6;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Files", Context); 
        }
    }
}



///new MenuItem { Name = Hardcoded.GetValue("Files", context), Url = AdminUrl("Storage/Files", siteDb) },