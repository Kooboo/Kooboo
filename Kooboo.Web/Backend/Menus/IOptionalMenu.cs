using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Backend.Menus
{
    public interface IOptionalMenu
    { 
        bool Show(RenderContext Context);
    }
}
