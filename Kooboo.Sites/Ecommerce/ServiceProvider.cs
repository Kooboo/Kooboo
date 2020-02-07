using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce
{
    public static class ServiceProvider
    {
        public static T GetService<T>(RenderContext Context) where T : IEcommerceService
        {
            var obj = Lib.IOC.Service.CreateInstanceByPriority<T>() as IEcommerceService;
            obj.Context = Context;
            obj.CommerceContext = GetCommerceContext(Context); 
            return (T)obj;
        }

        public static CommerceContext GetCommerceContext(RenderContext context)
        {
            CommerceContext commerceContext = new CommerceContext(context); 
            return commerceContext; 
        }

    }
}
