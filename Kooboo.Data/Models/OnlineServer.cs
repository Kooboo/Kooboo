//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.GeoLocation;
using System;
using System.Collections.Generic;
  
namespace Kooboo.Data.Models
{
    public class OnlineServer : IGolbalObject
    {
        private Guid _id; 

        public Guid Id {
            get
            {
                if (_id == default(Guid))
                {
                    if (ServerId !=default(int))
                    {
                        _id = Lib.Helper.IDHelper.NewIntGuid(ServerId); 
                    }
                    else if (!string.IsNullOrWhiteSpace(this.Name))
                    {
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Name); 
                    }
                }
                return _id; 
            }
            set
            {
                _id = value; 
            }
        }

        // the old server id.. 
        public int ServerId { get; set; }

        public string Name { get; set; }
         
        public string PrimaryIp
        {
            get;set;
        } 

        // intranet ip. 
        public string InternalIP { get; set; }

        public int SubMask { get; set; }
 
        public string Secondary
        {
            get;set;
        }

        [Obsolete]
        public string Country { get; set; }
         
        [Obsolete]
        public string ForCountry
        {
            get;set;
        }
          
        //Only for the server that act as DNS server. 
        public string NameServer
        {
            get;set;
        }

        public ServerType Type { get; set; }

        public int OrgCount { get; set; }

        public string HostDomain { get; set; }

        public bool IsPrivate { get; set; }

        public string PTR { get; set; }

        public int EmailServerId { get; set; }

        private Guid _privateorgid; 
        public Guid PrivateOrgId {
            get {

                if (_privateorgid == default(Guid))
                {
                    if (!string.IsNullOrWhiteSpace(PrivateOrgName))
                    {
                        _privateorgid = Lib.Security.Hash.ComputeGuidIgnoreCase(PrivateOrgName); 
                    }
                }
                return _privateorgid; 
            }
            set
            {
                _privateorgid = value; 
            }
        }
        
        // The agency name.....
        public string PrivateOrgName { get; set; }

        public int DesignOrgNumber { get; set; } = 999;
         
        [Obsolete]
        public string DataCenter
        {
            get;set;
        } 

        private Guid _hostdomainhash; 

        [Newtonsoft.Json.JsonIgnore]
        public Guid HostDomainHash
        {
           get
            {
                if (_hostdomainhash== default(Guid))
                {
                    if (!string.IsNullOrWhiteSpace(this.HostDomain))
                    {
                        _hostdomainhash = Lib.Security.Hash.ComputeGuidIgnoreCase(this.HostDomain); 
                    }
                }
                return _hostdomainhash; 
            }
        }

        private System.Net.IPAddress _primaryipaddress;
        [Newtonsoft.Json.JsonIgnore]
        public System.Net.IPAddress PrimaryIpAddress
        {
            get
            {
                if (_primaryipaddress == null)
                {
                    _primaryipaddress = System.Net.IPAddress.Parse(this.PrimaryIp.Trim());
                }
                return _primaryipaddress;
            }
        }
   
        private HashSet<string> _allips;
        [Newtonsoft.Json.JsonIgnore]
        public HashSet<string> AllIPS
        {
            get
            {
                if (_allips == null)
                {
                    _allips = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _allips.Add(this.PrimaryIp);

                    var otherips =  Helper.ServerHelper.ParseIps(this.Secondary);
                    foreach (var item in otherips)
                    {
                        _allips.Add(item);
                    }
                }
                return _allips;
            }
        }
         
        [Obsolete]
        [Newtonsoft.Json.JsonIgnore]
        public string Continent {
            get;set;
        }

        // the new data center id. 
        public int OnlineDataCenterId { get; set; }

        public override int GetHashCode()
        {
            string unique = this.DesignOrgNumber.ToString() + this.EmailServerId.ToString() + this.HostDomain;
            unique += this.Name + this.NameServer + this.OnlineDataCenterId.ToString();
            unique += this.OrgCount.ToString() + this.PrimaryIp.ToString() + this.PTR + this.Secondary;
            unique += this.SubMask + this.Type.ToString();
            unique += this.PrivateOrgName;
            unique += this.InternalIP;
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);  
        }
    }

    public enum ServerType
    {
        NormalHost = 0,
        Template = 1,
        AccountDns = 2,
        Mta = 4, 
        DnsOnly = 5,
        PrivateWeb = 6, 
        TBD = 9,
        Root=16, 
        Nginx = 32
        //0 = normal, 1= template, 2= accountDns, 4= wwwhost.  5. Nginx = front end proxy server. 
    }
    
}
