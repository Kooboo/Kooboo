//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Models
{
    public class BalanceChangeModel
    {
        public Guid UserId { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public Guid Id { get; set; }
    }
}