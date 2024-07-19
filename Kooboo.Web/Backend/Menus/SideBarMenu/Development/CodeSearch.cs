using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;

namespace Kooboo.Web.Backend.Menus.SideBarMenu.Development
{
    public class CodeSearch : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Development;

        public string Name => "Code search";

        public string Icon => "";

        public string Url => "Development/CodeSearch";

        public int Order => 12;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Code search", Context);
        }
    }
}
