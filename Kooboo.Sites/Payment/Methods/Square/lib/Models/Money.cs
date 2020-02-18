using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models
{
    public class Money
    {
        public Money(long? amount = null,
            string currency = null)
        {
            Amount = amount;
            Currency = currency;
        }

        /// <summary>
        /// The amount of money, in the smallest denomination of the currency
        /// indicated by `currency`. For example, when `currency` is `USD`, `amount` is
        /// in cents. Monetary amounts can be positive or negative. See the specific
        /// field description to determine the meaning of the sign in a particular case.
        /// </summary>
        [JsonProperty("amount")]
        public long? Amount { get; }

        /// <summary>
        /// Indicates the associated currency for an amount of money. Values correspond
        /// to [ISO 4217](https://wikipedia.org/wiki/ISO_4217).
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; }

        public Builder ToBuilder()
        {
            var builder = new Builder()
                .Amount(Amount)
                .Currency(Currency);
            return builder;
        }

        public class Builder
        {
            private long? amount;
            private string currency;

            public Builder() { }
            public Builder Amount(long? value)
            {
                amount = value;
                return this;
            }

            public Builder Currency(string value)
            {
                currency = value;
                return this;
            }

            public Money Build()
            {
                return new Money(amount,
                    currency);
            }
        }
    }
}
