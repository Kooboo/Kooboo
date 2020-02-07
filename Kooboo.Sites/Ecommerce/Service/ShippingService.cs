using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class ShippingService : IEcommerceService
    {
        public RenderContext Context { get; set; }
        public CommerceContext CommerceContext { get; set; }

        public long Priority => 1;

        public decimal CalculateCost(Cart cart)
        {
            return (decimal)2.95;
        }
    }
}
