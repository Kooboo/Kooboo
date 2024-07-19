using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;

namespace Kooboo.Web.Backend.Menus.SideBarMenu.Development
{
    public class SpaMultilingual : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Development;

        public string Name => "SpaMultilingual";

        public string Icon => "";

        public string Url => "Development/SpaMultilingual";

        public int Order => 18;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Spa multilingual", Context);
        }
    }
}
