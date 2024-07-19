using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.FeatureMenus
{
    public class Modules : IFeatureMenu
    {

        public string Name => "Modules";

        public string Icon => "";

        public string Url => "System/Modules";

        public int Order => 9;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Modules", Context);
        }
    }
}

