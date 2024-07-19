using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;

namespace Kooboo.Web.Backend.Menus.SideBarMenu.Database
{
    public class Logging : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Database;

        public string Name => "SQL logs";

        public string Icon => "";

        public string Url => "Storage/Logging";

        public int Order => 100;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("SQL logs", Context);
        }
    }
}
