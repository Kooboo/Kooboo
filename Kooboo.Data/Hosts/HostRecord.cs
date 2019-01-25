//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Hosts
{
  public  class HostRecord
    {
      /// <summary>
      /// The full domain record that appear in the host file, including subdomains. 
      /// </summary>
      public string Domain { get; set; }

      /// <summary>
      /// The ip address of this host record.
      /// </summary>
      public string IpAddress { get; set; }

      /// <summary>
      /// The original line string that parsed into records. 
      /// </summary>
      public string LineString { get; set; }

      public static HostRecord  Parse(string line)
      {
          if (string.IsNullOrWhiteSpace(line))
          {
              return null; 
          }

          line = line.Trim(); 

          HostRecord record = new HostRecord();

          int spaceindex = line.IndexOf(' ');

          if (spaceindex > 0)
          {
              
              string IPstring = line.Substring(0, spaceindex);

              System.Net.IPAddress outIpAddress;

              bool test = System.Net.IPAddress.TryParse(IPstring, out outIpAddress);
              if (test)
              {
                  record.IpAddress = IPstring;

                  record.Domain = line.Substring(spaceindex).Trim(); 
                  record.LineString = line;
                  return record;
              }
          }

       

          return null; 
      }

    }
}
