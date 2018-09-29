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

        public int SubMask { get; set; }
 
        public string Secondary
        {
            get;set;
        }

        public string Country { get; set; }

        private string _forcountry;
        public string ForCountry
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_forcountry))
                {
                    return this.Country;
                }
                else
                {
                    return _forcountry;
                }
            }
            set { _forcountry = value; }
        }

        public string State { get; set; }
         
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
        
        public string PrivateOrgName { get; set; }

        public int DesignOrgNumber { get; set; } = 999;
         
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


        private string _continent; 

        [Newtonsoft.Json.JsonIgnore]
        public string Continent {
            get
            { 
                if (string.IsNullOrWhiteSpace(_continent))
                { 
                   if (!string.IsNullOrEmpty(this.ForCountry))
                    {
                        var countryCont =  CountryLocation.FindCountryLocation(this.ForCountry);
                        if (countryCont != null && !string.IsNullOrWhiteSpace(countryCont.Continent))
                        {
                            _continent = countryCont.Continent;
                        }
                        else
                        { _continent = "ZZ"; }
                    }
                   else
                    {
                        _continent = "ZZ"; 
                    }
                     
                }

                return _continent; 
            }

            set
            {
                _continent = value; 
            }
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
        Root=16
        //0 = normal, 1= template, 2= accountDns, 4= wwwhost. 
    }
}
