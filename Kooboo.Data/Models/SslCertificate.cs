//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
 public   class SslCertificate : IGolbalObject
    {
         
        private Guid _id;
        public Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(Domain);
                }
                return _id;
            }
        }

        public string Domain { get; set; }

        public  bool IsWildCard { get; set; }

        public DateTime Expiration { get; set; }
 
        public byte[] Content { get; set; }
         
        public Guid OrganizationId { get; set; }

        public override int GetHashCode()
        {

            string unique = this.Domain + this.IsWildCard.ToString() + this.Expiration.ToLongTimeString() + this.OrganizationId.ToString(); 

            if (Content !=null)
            {
                unique += System.Text.Encoding.UTF8.GetString(Content); 
            }
             
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

    }
}
