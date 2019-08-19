using Kooboo.Data.Context;
using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Menus
{
    // When a menu is show based on certain conditions. 
    // Like Events... show when enable.. 
    public interface IDynamicMenu
    {
        List<ICmsMenu> ShowSubItems(RenderContext context); 
        bool Show(RenderContext context);  
    }
}
