//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
   public class SiteErrorLog
    {
        private Guid _id; 

        public Guid Id {
            get {
                if (_id == default(Guid))
                {
                    if (!string.IsNullOrEmpty(this.Url))
                    {
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Url); 
                    }
                }
                return _id; 
            }
            set { _id = value;  }
        }


        public string Url { get; set; }
          
        public string ClientIP { get; set; }

        public DateTime StartTime { get; set; } = DateTime.UtcNow; 

        public string Message { get; set; }

        public int StatusCode { get; set; }
    }
}
