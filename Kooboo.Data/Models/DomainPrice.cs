//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class DomainPrice : IGolbalObject
    {
        private Guid _id; 
        public Guid Id {
            get
            {
                if (_id == default(Guid))
                {
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Tld); 
                }
                return _id; 
            }
            set
            {
                _id = value; 
            }
        }

        private string _tld; 

        public string Tld {
            get { return _tld;  }

            set
            {
                _tld = value; 
                if (!string.IsNullOrWhiteSpace(_tld))
                {
                    _tld = _tld.Trim(); 
                }

            }
        }

        public decimal YearlyPrice { get; set; }

        public string Currency { get; set; }

    }
}
