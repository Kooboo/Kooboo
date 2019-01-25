//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
  public  class DataString
    {
        public string Base64String { get; set; }
         
        private Guid _hash; 

        public Guid Hash {
            get
            {
                if (_hash == default(Guid) && !string.IsNullOrEmpty(this.Base64String))
                {
                    _hash = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Base64String); 
                }
                return _hash; 
            }
            set { _hash = value;  }
        }

        public bool Validate()
        {
            var hash = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Base64String);

            return hash == this.Hash; 
        }
    }
}
