//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
  
namespace Kooboo.Data.Models
{

    [Serializable]
    public class Binding : IGolbalObject
    { 
        private Guid _id;
        public Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    string unique = this.DomainId.ToString().ToLower() + this.SubDomain + this.Device; 
                    if (this.DefaultPortBinding)
                    {
                        unique += this.Port.ToString(); 
                    }

                    _id = Lib.Security.Hash.ComputeHashGuid(unique);  
                }
                return _id;
            }
        }

        public Guid OrganizationId { get; set; }

        /// <summary>
        ///  the name key of the website. used as a foreign key. 
        /// </summary>
        // public string websiteName;
        public Guid WebSiteId;

        /// <summary>
        /// Subdomain, includes www or others. 
        /// </summary>
        public string SubDomain { get; set; }

        public Guid DomainId { get; set; }

        /// <summary>
        /// The full domain record including sub domain information
        /// </summary>
        public string FullName
        {
            get
            {
                if (DomainId != default(Guid))
                {
                    var domain = Kooboo.Data.GlobalDb.Domains.Get(DomainId);
                    if (domain == null)
                    {
                        return string.Empty;
                    }

                    if (string.IsNullOrEmpty(SubDomain))
                    {
                        if (domain.DomainName.StartsWith("."))
                        {
                            return domain.DomainName.Substring(1);
                        }
                        else
                        {
                            return domain.DomainName;
                        }
                    }
                    return SubDomain + "." + domain.DomainName;
                }


                if (DefaultPortBinding && Port >0)
                {
                    string ip = Kooboo.Lib.Helper.NetworkHelper.GetLocalIpAddress();
                    return ip + ":" + Port.ToString();  
                }

                return null; 
            }
        }
         
        /// <summary>
        /// Device is the user agent. It used a contains to match the user agent.
        /// blank = match all. 
        /// </summary>
        public string Device { get; set; }

        private string _ipaddress;

        /// <summary>
        ///  if this binding is bind to an external ip, this is the ip address.
        /// </summary>
        public string IpAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_ipaddress))
                {
                    return "127.0.0.1";
                }
                return _ipaddress;
            }
            set
            {
                _ipaddress = value;
            }
        }

        // for default port binding..
        public int Port { get; set; } = 0; 
         
        public bool DefaultPortBinding { get; set;  }

        public override int GetHashCode()
        {
            string unique = this.OrganizationId.ToString() + this.WebSiteId.ToString();
            unique += this.IpAddress; 
             return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }
}
