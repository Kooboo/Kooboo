namespace Kooboo.Mail.Spf
{
    public enum ReturnCode : ushort
    {
        /// <summary>
        ///   <para>No error</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc1035.html">RFC 1035</a>.
        ///   </para>
        /// </summary>
        NoError = 0,

        /// <summary>
        ///   <para>Format error</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc1035.html">RFC 1035</a>.
        ///   </para>
        /// </summary>
        FormatError = 1,

        /// <summary>
        ///   <para>Server failure</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc1035.html">RFC 1035</a>.
        ///   </para>
        /// </summary>
        ServerFailure = 2,

        /// <summary>
        ///   <para>Non-existent domain</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc1035.html">RFC 1035</a>.
        ///   </para>
        /// </summary>
        NxDomain = 3,

        /// <summary>
        ///   <para>Not implemented</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc1035.html">RFC 1035</a>.
        ///   </para>
        /// </summary>
        NotImplemented = 4,

        /// <summary>
        ///   <para>Query refused</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc1035.html">RFC 1035</a>.
        ///   </para>
        /// </summary>
        Refused = 5,

        /// <summary>
        ///   <para>Name exists when it should not</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2136.html">RFC 2136</a>.
        ///   </para>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        YXDomain = 6,

        /// <summary>
        ///   <para>Record exists when it should not</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2136.html">RFC 2136</a>.
        ///   </para>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        YXRRSet = 7,

        /// <summary>
        ///   <para>Record that should exist does not</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2136.html">RFC 2136</a>.
        ///   </para>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        NXRRSet = 8,

        /// <summary>
        ///   <para>Server is not authoritative for zone</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2136.html">RFC 2136</a>.
        ///   </para>
        /// </summary>
        NotAuthoritive = 9,

        /// <summary>
        ///   <para>Name not contained in zone</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2136.html">RFC 2136</a>.
        ///   </para>
        /// </summary>
        NotZone = 10,

        /// <summary>
        ///   <para>EDNS version is not supported by responder</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2671.html">RFC 2671</a>
        ///     and <a href="https://www.rfc-editor.org/rfc/rfc6891.html">RFC 6891</a>.
        ///   </para>
        /// </summary>
        BadVersion = 16,

        /// <summary>
        ///   <para>TSIG signature failure</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2845.html">RFC 2845</a>.
        ///   </para>
        /// </summary>
        BadSig = 16,

        /// <summary>
        ///   <para>Key not recognized</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2845.html">RFC 2845</a>.
        ///   </para>
        /// </summary>
        BadKey = 17,

        /// <summary>
        ///   <para>Signature out of time window</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2845.html">RFC 2845</a>.
        ///   </para>
        /// </summary>
        BadTime = 18,

        /// <summary>
        ///   <para>Bad TKEY mode</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2930.html">RFC 2930</a>.
        ///   </para>
        /// </summary>
        BadMode = 19,

        /// <summary>
        ///   <para>Duplicate key name</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2930.html">RFC 2930</a>.
        ///   </para>
        /// </summary>
        BadName = 20,

        /// <summary>
        ///   <para>Algorithm not supported</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc2930.html">RFC 2930</a>.
        ///   </para>
        /// </summary>
        BadAlg = 21,

        /// <summary>
        ///   <para>Bad truncation of TSIG record</para>
        ///   <para>
        ///     Defined in
        ///     <a href="https://www.rfc-editor.org/rfc/rfc4635.html">RFC 4635</a>.
        ///   </para>
        /// </summary>
        BadTrunc = 22,

        /// <summary>
        ///   <para>Bad/missing server cookie</para>
        ///   <para>
        ///     Defined in
        ///     <a href="http://tools.ietf.org/html/draft-ietf-dnsop-cookies">draft-ietf-dnsop-cookies</a>
        ///   </para>
        /// </summary>
        BadCookie = 23
    }
}

