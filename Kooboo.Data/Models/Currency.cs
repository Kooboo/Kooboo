//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kooboo.Data.Models
{
    public class Currency : IGolbalObject
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {

                    if (!string.IsNullOrWhiteSpace(this.Code) && !string.IsNullOrWhiteSpace(StandardMoneyCode))
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

        public string StandardMoneyCode { get; set; }

        public string Code;

        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Code);
            }
        }

        public decimal MarketRate { get; set; }

        //this Rate Can be modified by us.
        public decimal Rate { get; set; }

        public decimal ServiceChargeRate { get; set; } = 0.0M;


        public override int GetHashCode()
        {
            string unique = this.MarketRate.ToString() + this.Rate.ToString() + this.ServiceChargeRate.ToString();

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}
