using System; 

namespace Kooboo.Data.Models
{
    public class SalesItem
    {
        // domain, template, package, balance... 
        public string ProductTypeName { get; set; }

        // tempmlate id, domain name(kooboo.com), package id. 
        public string ProductKey { get; set; }

        public decimal Quantity { get; set; }
    }
}
