using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public class TransferResponse : BaseResponse
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public Money Amount { get; set; }
        public DateTime Created { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public string CorrelationId { get; set; }
        public Uri TransferURL { get; set; }
    }
}
