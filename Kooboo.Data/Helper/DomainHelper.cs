//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Nager.PublicSuffix;

namespace Kooboo.Data.Helper
{
    public static class DomainHelper
    {
        static DomainHelper()
        {
            parser = new DomainParser();
        }

        private static DomainParser parser { get; set; }

        public static DomainResult Parse(string fullDomain)
        {
            DomainResult result = new DomainResult();
            if (string.IsNullOrWhiteSpace(fullDomain))
            {
                return result;
            }

            if (fullDomain.ToLower().EndsWith(".kooboo"))
            {
                result.Domain = "kooboo";
                result.SubDomain = fullDomain.Substring(0, fullDomain.Length - 7);
                return result;
            }

            var parserResult = parser.Get(fullDomain);
            if (parserResult != null)
            {
                result.SubDomain = parserResult.SubDomain;
                result.Domain = parserResult.RegistrableDomain;
            }
            return result;
        }

        public static string GetRootDomain(string fullDomain)
        {
            var result = Parse(fullDomain);
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

            return SubDomain + "." + Domain;
        }
    }
}