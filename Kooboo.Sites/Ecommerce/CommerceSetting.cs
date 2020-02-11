using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce
{
    public class CommerceSetting : ISiteSetting
    {
        public string Name =>"Commerce";

        public decimal DefaultShippingCost { get; set; }

        public string Currency { get; set; }

    }
}
