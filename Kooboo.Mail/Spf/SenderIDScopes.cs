namespace Kooboo.Mail.Spf
{
    /// <summary>
    /// SenderIDCheckHostArgument
    /// <see href="https://datatracker.ietf.org/doc/html/rfc4406#section-4.1"/>
    /// </summary>
    public enum SenderIDScopes
    {
        /// <summary>
        /// "mfrom" (for the MAIL FROM variant of the test)
        /// </summary>
        MFrom,

        /// <summary>
        /// A scope of "pra" (for the PRA variant of the test),
        /// </summary>
        Pra,

        /// <summary>
        /// other scene, unknown tag
        /// </summary>
        Unknown
    }
}

