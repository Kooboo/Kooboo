using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public class KpSessionRequest
    {
        [JsonProperty("purchase_country")]
        public string PurchaseCountry { get; set; }

        [JsonProperty("purchase_currency")]
        public string PurchaseCurrency { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Non-negative, minor units. Total amount of the order, including tax and any discounts.
        /// </summary>
        [JsonProperty("order_amount")]
        public long OrderAmount { get; set; }

        [JsonProperty("order_tax_amount")]
        public long OrderTaxAmount { get; set; }

        [JsonProperty("order_lines")]
        public OrderLine[] OrderLines { get; set; }

        //[JsonProperty("acquiring_channel")]
        //public string AcquiringChannel => "IN_STORE";

        public class OrderLine
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("reference")]
            public string Reference { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("quantity")]
            public int Quantity { get; set; }

            [JsonProperty("unit_price")]
            public long UnitPrice { get; set; }

            [JsonProperty("tax_rate")]
            public double TaxRate { get; set; }

            [JsonProperty("total_amount")]
            public long TotalAmount { get; set; }

            [JsonProperty("total_discount_amount")]
            public long TotalDiscountAmount { get; set; }

            [JsonProperty("total_tax_amount")]
            public long TotalTaxAmount { get; set; }
        }
    }
}