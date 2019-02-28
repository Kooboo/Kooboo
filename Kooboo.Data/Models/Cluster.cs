//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Models
{
   public class Cluster : IGolbalObject
    {
        private Guid _id; 
        public Guid Id {
            get {
                if (_id == default(Guid))
                {     string unique = this.WebSiteId.ToString() + this.OrganizationId.ToString() + this.ServerId.ToString();
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(unique);  
                }
                return _id; 
            }
            set
            {
                _id = value; 
            }
        }

        public Guid WebSiteId { get; set; }

        public Guid OrganizationId { get; set; }

        public int ServerId  { get; set; }

        public string PrimaryDomain { get; set; }

        public string FullDomains { get; set;  }

        private List<string> _domains; 
        public List<string> Domains
        {
            get
            {
               if (_domains == null)
                {  
                    if (!string.IsNullOrEmpty(FullDomains))
                    {
                        _domains = new List<string>();  
                        string sep = ",;";

                        string[] domains = FullDomains.Split(sep.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        foreach (var item in domains)
                        {
                            _domains.Add(item.ToLower()); 
                        } 
                    }

                    if (!string.IsNullOrEmpty(this.PrimaryDomain))
                    {
                        if (_domains == null)
                        {
                            _domains = new List<string>();
                        }

                        _domains.Add(this.PrimaryDomain.ToLower()); 

                    }
                }

                return _domains; 
            }
        }

        public override int GetHashCode()
        {
            string unique = this.FullDomains + this.PrimaryDomain + this.ServerId.ToString() + this.WebSiteId.ToString() + this.OrganizationId.ToString();

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }
}
