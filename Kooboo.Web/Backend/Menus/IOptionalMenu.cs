using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Backend.Menus
{
    // When a menu is show based on certain conditions. 
    // Like Events... show when enable.. 
    public interface IOptionalMenu
    { 
        bool Show(RenderContext Context);
    }
}
