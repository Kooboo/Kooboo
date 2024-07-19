using System.Net;

namespace Kooboo.Mail.Spf
{
    /// <summary>
    /// Check host function argument
    /// <see href="https://datatracker.ietf.org/doc/html/rfc7208#section-4.1"/>
    /// </summary>
    public class CheckHostArgument
    {
        public CheckHostArgument(IPAddress clientAddress, string domain, string sender)
        {
            LoopCount = 0;
            IP = clientAddress;
            Domain = domain;
            Sender = sender;
        }

        public CheckHostArgument(string newDomain, CheckHostArgument oldArgument)
        {
            LoopCount = oldArgument.LoopCount++;
            Domain = newDomain;
            Sender = oldArgument.Sender;
            IP = oldArgument.IP;
        }

        /// <summary>
        /// the IP address of the SMTP client that is emitting the mail, either IPv4 or IPv6.
        /// </summary>
        public IPAddress IP { get; set; }

        /// <summary>
        /// the domain that provides the sought-after authorization information; initially, the domain portion of the "MAIL FROM" or "HELO" identity.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// the "MAIL FROM" or "HELO" identity.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// recurrent calculate count
        /// </summary>
        public int LoopCount { get; set; }
    }
}

