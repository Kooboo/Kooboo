namespace Kooboo.Lib.Domain
{

    public record DomainResult
    {
        public string Tld;
        public string Root;

        public string SubDomainRoot;

        public string FullSubDomain;

        public string FullName;
    }
}
