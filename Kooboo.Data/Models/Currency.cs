using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class Currency: IGolbalObject
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    
                    if (!string.IsNullOrWhiteSpace(this.Code)&&!string.IsNullOrWhiteSpace(StandardMoneyCode))
                    {
                        var key = this.Code + this.StandardMoneyCode;
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(key);
                    }
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Code;

        public string Name { get; set; }

        public string StandardMoneyCode { get; set; }

        public decimal MarketExchangeRate { get; set; }

        public decimal ReverseMarketExchangeRate { get; set; }

        public decimal ExchangeRate { get; set; }
        
    }
}
