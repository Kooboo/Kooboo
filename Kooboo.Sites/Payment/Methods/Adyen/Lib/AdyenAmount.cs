using System;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Adyen.Lib
{
    public class AdyenAmount
    {
        /// <summary>
        /// amount in minor units
        /// </summary>
        [JsonProperty("value")]
        public long Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        public static long FormatAmountToMinorUnits(string currency, decimal amount)
        {
            var uppperCurrency = currency.ToUpper();
            switch (uppperCurrency)
            {
                case "CVE":
                case "IDR":
                    return (long)amount;
                default:
                    return CurrencyDecimalPlaceConverter.ToMinorUnit(currency, amount);
            }
        }
    }
}
