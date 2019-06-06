//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved. 
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Menus
{
    public interface ICmsMenu
    {
        string Name { get;  }

        string GetDisplayName(RenderContext Context);
         
        string Icon { get;  }

        string Url { get;  }
 

        int Order { get;   } 

        List<ICmsMenu> SubItems { get; set; }

    }
}
