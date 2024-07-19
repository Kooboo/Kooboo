namespace Kooboo.Mail.Spf
{
    /// <summary>
    /// Results of Evaluation
    /// <see href="https://datatracker.ietf.org/doc/html/rfc7208#section-2.6"/>
    /// </summary>
    public enum ResultsOfEvaluation
    {
        /// <summary>
        /// A result of "none" means either (a) no syntactically valid DNS domain name was extracted from
        /// the SMTP session that could be used as the one to be authorized, or (b) no SPF records were
        /// retrieved from the DNS.
        /// </summary>
        None,

        /// <summary>
        /// A "neutral" result means the ADMD has explicitly stated that it is not asserting whether the IP address is authorized.
        /// </summary>
        Neutral,

        /// <summary>
        /// A "neutral" result means the ADMD has explicitly stated that it is not asserting whether the IP address is authorized.
        /// </summary>
        Pass,

        /// <summary>
        /// A "neutral" result means the ADMD has explicitly stated that it is not asserting whether the IP address is authorized.
        /// </summary>
        Fail,

        /// <summary>
        /// A "softfail" result is a weak statement by the publishing ADMD that the host is probably not authorized.
        /// It has not published a stronger, more definitive policy that results in a "fail".
        /// </summary>
        Softfail,

        /// <summary>
        /// A "temperror" result means the SPF verifier encountered a transient (generally DNS) error while performing the check.
        /// A later retry may succeed without further DNS operator action.
        /// </summary>
        Temperror,

        /// <summary>
        /// A "temperror" result means the SPF verifier encountered a transient (generally DNS) error while performing the check.
        /// A later retry may succeed without further DNS operator action.
        /// </summary>
        Permerror,
    }
}

