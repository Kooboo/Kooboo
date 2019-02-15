using Kooboo.Api;
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.CmsMenu
{
    public interface IMenu
    {
        string Name { get;  }

        string GetDisplayName(RenderContext Context);

        string Icon { get;  }

        string Url { get;  }

        int Order { get;   }

        bool CanShow(RenderContext context);

        List<IMenu> Items
        {
            get; set;
        }
    }
}
