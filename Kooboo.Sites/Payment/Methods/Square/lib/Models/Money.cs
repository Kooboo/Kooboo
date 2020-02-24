using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models
{
    public class Money
    {
        /// <summary>
        /// The amount of money, in the smallest denomination of the currency
        /// indicated by `currency`. For example, when `currency` is `USD`, `amount` is
        /// in cents. Monetary amounts can be positive or negative. See the specific
        /// field description to determine the meaning of the sign in a particular case.
        /// </summary>
        [JsonProperty("amount")]
        public long? Amount { get; set; }

        /// <summary>
        /// Indicates the associated currency for an amount of money. Values correspond
        /// to [ISO 4217](https://wikipedia.org/wiki/ISO_4217).
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
