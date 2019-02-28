//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Extensions;
using System.Net; 

namespace Kooboo.Lib.Utilities
{
   public  class NetworkSetting
    {

       public int Id
       {
           get
           { 
                return Lib.Security.Hash.ComputeInt(this.MACAddress);  
           }
       }

       /// <summary>
       /// the mac address of current netowrk. 
       /// </summary>
       public string MACAddress { get; set; }

       /// <summary>
       /// the Setting Id of this network interface. 
       /// </summary>
       public string SettingID { get; set; }

       public Guid SettingGuid
       {
           get
           {
               return System.Guid.Parse(this.SettingID); 
           }
       }

       /// <summary>
       /// primary DNS server. 
       /// </summary>
       public string PrimaryDnsServer { get; set; }

       public string SecondDnsServer { get; set; }

       /// <summary>
       /// dymanic or static dns server setting for this network interface. 
       /// </summary>
       public bool AutomaticDns { get; set; }
        
    }
}
