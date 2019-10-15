using Kooboo.Data.Context;
using System.Collections.Generic;

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