//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using System.Collections.Generic;

namespace Kooboo.Data.ViewModel
{
    public class DomainPriceViewModel
    {
        public string Domain { get; set; }

        private decimal _price;

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                Options = new List<DomainOption>();

                for (var i = 1; i <= 3; i++)
                {
                    Options.Add(new DomainOption()
                    {
                        Price = _price * i,
                        Year = i
                    });
                }
            }
        }

        public Currency Currency { get; set; }

        public List<DomainOption> Options { get; set; }
    }

    public class DomainOption
    {
        public decimal Price { get; set; }

        public int Year { get; set; }
    }
}