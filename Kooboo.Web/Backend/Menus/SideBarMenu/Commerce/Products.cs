using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Commerce
{
    [Obsolete]
    public class Products : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Commerce;

        public string Name => "Products";

        public string Icon => "";

        public string Url => "ECommerce/Products";

        public int Order => 0;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Products", Context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Code",context), Url = AdminUrl("Development/Code", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Code }