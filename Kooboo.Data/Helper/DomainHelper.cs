//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Nager.PublicSuffix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
 public static  class DomainHelper
    {

        static DomainHelper()
        {
           parser = new DomainParser();
        }

        private static DomainParser parser { get; set; }

        public static DomainResult Parse(string FullDomain)
        {
            DomainResult result = new DomainResult(); 
            if (string.IsNullOrWhiteSpace(FullDomain))
            {
                return result; 
            }

            if (FullDomain.ToLower().EndsWith(".kooboo"))
            {
                result.Domain = "kooboo";
                result.SubDomain = FullDomain.Substring(0, FullDomain.Length - 7);
                return result; 
            }

            var parserResult = parser.Get(FullDomain); 
            if (parserResult != null)
            {
                result.SubDomain = parserResult.SubDomain;
                result.Domain = parserResult.RegistrableDomain; 
            }
            return result; 
        }

        public static string GetRootDomain(string FullDomain)
        {
            var result = Parse(FullDomain);
            return result.Domain; 
        }
                    

    }

    public class DomainResult
    {
        public string Domain { get; set; }
        public string SubDomain { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.SubDomain))
            {
                return this.Domain; 
            }

            else
            {
                return SubDomain + "." + Domain; 
             } 
        }
    }
}
