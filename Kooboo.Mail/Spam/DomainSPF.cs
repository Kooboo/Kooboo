using System.Collections.Generic;

namespace Kooboo.Mail.Spam
{
    public class DomainSPF
    {
        public List<string> IPV4 { get; set; }

        public List<string> IPV6 { get; set; }
    }



    public record SPFResult
    {
        public List<string> IPV4 { get; set; } = new List<string>();

        public List<string> IPV6 { get; set; } = new List<string>();


    }

}
