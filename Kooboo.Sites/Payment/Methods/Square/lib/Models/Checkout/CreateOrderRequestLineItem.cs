using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout
{
    public class CreateOrderRequestLineItem
    {
        /// <summary>
        /// Only used for ad hoc line items. The name of the line item. This value cannot exceed 500 characters.
        /// Do not provide a value for this field if you provide a value for `catalog_object_id`.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The quantity to purchase, as a string representation of a number.
        /// This string must have a positive integer value.
        /// </summary>
        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// Represents an amount of money. `Money` fields can be signed or unsigned.
        /// Fields that do not explicitly define whether they are signed or unsigned are
        /// considered unsigned and can only hold positive amounts. For signed fields, the
        /// sign of the value indicates the purpose of the money transfer. See
        /// [Working with Monetary Amounts](https://developer.squareup.com/docs/build-basics/working-with-monetary-amounts)
        /// for more information.
        /// </summary>
        [JsonProperty("base_price_money")]
        public Models.Money BasePriceMoney { get; set; }

        ///// <summary>
        ///// Only used for ad hoc line items. The variation name of the line item. This value cannot exceed 255 characters.
        ///// If this value is not set for an ad hoc line item, the default value of `Regular` is used.
        ///// Do not provide a value for this field if you provide a value for the `catalog_object_id`.
        ///// </summary>
        //[JsonProperty("variation_name")]
        //public string VariationName { get; }

        /// <summary>
        /// The note of the line item. This value cannot exceed 500 characters.
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; }

        ///// <summary>
        ///// Only used for Catalog line items.
        ///// The catalog object ID for an existing [CatalogItemVariation](#type-catalogitemvariation).
        ///// Do not provide a value for this field if you provide a value for `name` and `base_price_money`.
        ///// </summary>
        //[JsonProperty("catalog_object_id")]
        //public string CatalogObjectId { get; }

        ///// <summary>
        ///// Only used for Catalog line items. The modifiers to include on the line item.
        ///// </summary>
        //[JsonProperty("modifiers")]
        //public IList<CreateOrderRequestModifier> Modifiers { get; }

        ///// <summary>
        ///// The taxes to include on the line item.
        ///// </summary>
        //[JsonProperty("taxes")]
        //public IList<CreateOrderRequestTax> Taxes { get; }

        ///// <summary>
        ///// The discounts to include on the line item.
        ///// </summary>
        //[JsonProperty("discounts")]
        //public IList<CreateOrderRequestDiscount> Discounts { get; }
    }

    public class CreateOrderRequestModifier
    {
    }

    public class CreateOrderRequestTax
    {
    }

    public class CreateOrderRequestDiscount
    {
    }
}
