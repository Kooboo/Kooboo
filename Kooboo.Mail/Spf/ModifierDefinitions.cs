namespace Kooboo.Mail.Spf
{
    /// <summary>
    /// <see href="https://datatracker.ietf.org/doc/html/rfc7208#section-6"/>
    /// </summary>
    public enum ModifierDefinitions
    {
        /// <summary>
        /// The "redirect" modifier is intended for consolidating both authorizations and policy into a common set to be shared within a single ADMD.
        /// It is possible to control both authorized hosts and policy for an arbitrary number of domains from a single record.
        /// </summary>
        Redirect,

        /// <summary>
        /// If check_host() results in a "fail" due to a mechanism match (such as "-all"), and the "exp" modifier is present,
        /// then the explanation string returned is computed as described below. If no "exp" modifier is present,
        /// then either a default explanation string or an empty explanation string MUST be returned to the calling application.
        /// </summary>
        Exp,

        /// <summary>
        /// other scene,unknown tag
        /// </summary>
        Unknown
    }
}

