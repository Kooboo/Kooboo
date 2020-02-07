using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Service
{
    public interface IEcommerceService : Kooboo.Lib.IOC.IPriority
    {
          RenderContext Context { get; set; }

          CommerceContext CommerceContext { get; set; }
    }
}
