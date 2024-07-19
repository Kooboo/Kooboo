using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;

namespace Kooboo.Web.Backend.Menus.SideBarMenu.Development
{
    public class OpenApis : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Development;

        public string Name => "OpenApis";

        public string Icon => "";

        public string Url => "Development/OpenApis";

        public int Order => 10;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("OpenApis", Context);
        }
    }
}
