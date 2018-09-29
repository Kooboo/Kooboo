using System.Collections.Generic;

namespace Nager.PublicSuffix
{
    internal class DomainDataStructure
    {
        public string Domain { get; internal set; }
        public TldRule TldRule { get; internal set; }
        public Dictionary<string, DomainDataStructure> Nested { get; internal set; }

        public DomainDataStructure(string domain)
        {
            this.Domain = domain;
            this.Nested = new Dictionary<string, DomainDataStructure>();
        }

        public DomainDataStructure(string domain, TldRule tldRule)
        {
            this.Domain = domain;
            this.TldRule = tldRule;
            this.Nested = new Dictionary<string, DomainDataStructure>();
        }
    }
}
