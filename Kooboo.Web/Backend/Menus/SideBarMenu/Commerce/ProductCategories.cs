using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.Commerce
{
    public class ProductCategories : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Commerce;

        public string Name => "ProductCategories";

        public string Icon => "";

        public string Url => "ECommerce/Product/Categories";

        public int Order => 2;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Product Categories", context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Code",context), Url = AdminUrl("Development/Code", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Code }