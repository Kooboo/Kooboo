using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Backend.Menus.SideBarMenu.Commerce
{
   
    public class ShoppingCart : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Commerce;

        public string Name => "ShoppingCart";

        public string Icon => "";

        public string Url => "ECommerce/shoppingcart";

        public int Order => 0;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Shopping Cart", Context);
        }
    }



}
