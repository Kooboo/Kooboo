using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{ 
    public  class DNSRecord : IGolbalObject
    {

        private Guid _id;  
        public Guid Id {
            get {
                if (_id == default(Guid))
                {
                    _id = Guid.NewGuid(); 
                }
                return _id; 
            }
            set { _id = value;  }
        }

        private Guid _domainid; 
        public Guid DomainId {
            get {
                if (_domainid == default(Guid))
                {
                    if (!string.IsNullOrWhiteSpace(this.Domain))
                    {
                        _domainid = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Domain); 
                    }
                }
                return _domainid; 
            }
            set
            {
                _domainid = value; 
            }
        }

        public string Host { get; set; }

        public string Value { get; set; }

        public string Type { get; set; } = "A";

       
        public string Domain { get; set; }

        public int Priority { get; set; }

        public override int GetHashCode()
        {
            string unique = this.DomainId.ToString() + this.Host + this.Value + this.Type + this.Priority.ToString(); 
            return base.GetHashCode();
        }
    }



}
