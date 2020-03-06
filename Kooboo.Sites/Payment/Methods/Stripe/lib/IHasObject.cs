using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Stripe.lib
{
    /// <summary>
    /// Interface that identifies entities returned by Stripe that have an `object` attribute.
    /// </summary>
    public interface IHasObject
    {
        /// <summary>
        /// String representing the object's type. Objects of the same type share the same value.
        /// </summary>
        string Object { get; set; }
    }
}
