using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.Commerce
{
    public class ProductTypes : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Commerce;

        public string Name => "ProductTypes";

        public string Icon => "";

        public string Url => "ECommerce/Product/Types";

        public int Order => 1;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Product Types", context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Code",context), Url = AdminUrl("Development/Code", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Code }