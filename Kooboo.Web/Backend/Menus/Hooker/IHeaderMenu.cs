using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Backend.Menus.Hooker
{
    interface IHeaderMenu
    {
        CmsMenuViewModel[] Append();  
        
    }
}
