namespace Kooboo.Mail.Spf
{
    /// <summary>
    /// Mechanism Definitions
    /// <see href="https://datatracker.ietf.org/doc/html/rfc7208#section-5"/>
    /// </summary>
    public enum MechanismDefinitions
    {
        /// <summary>
        /// The "all" mechanism is a test that always matches. It is used as the rightmost mechanism in a record to provide an explicit default.
        /// </summary>
        All,

        /// <summary>
        /// The "include" mechanism makes it possible for one domain to designate multiple administratively independent domains. 
        /// For example, a vanity domain "example.net" might send mail using the servers of administratively independent domains example.com and example.org.
        /// </summary>
        Include,

        /// <summary>
        /// This mechanism matches if ip is one of the target-name's IP addresses. For clarity, this means the "a" mechanism also matches AAAA records.
        /// </summary>
        A,

        /// <summary>
        /// This mechanism matches if ip is one of the MX hosts for a domain name.
        /// </summary>
        Mx,

        /// <summary>
        /// This mechanism tests whether the DNS reverse-mapping for ip exists and correctly points to a domain name within a particular domain.
        /// This mechanism SHOULD NOT be published. See the note at the end of this section for more information.
        /// </summary>
        Ptr,

        /// <summary>
        /// These mechanisms test whether ip is contained within a given IP network.
        /// </summary>
        Ip4,

        /// <summary>
        /// These mechanisms test whether ip is contained within a given IP network.
        /// </summary>
        Ip6,

        /// <summary>
        /// This mechanism is used to construct an arbitrary domain name that is used for a DNS A record query.
        /// It allows for complicated schemes involving arbitrary parts of the mail envelope to determine what is permitted.
        /// </summary>
        Exists,

        /// <summary>
        /// other scene, unknown tag
        /// </summary>
        Unknown
    }
}

