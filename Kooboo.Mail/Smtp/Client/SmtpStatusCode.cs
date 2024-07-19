//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Smtp
{
    // Summary:
    //     Specifies the outcome of sending e-mail by using the System.Net.Mail.SmtpClient
    //     class.
    public enum SmtpStatusCode
    {
        /// <summary>
        /// The transaction could not occur. You receive this error when the specified SMTP host cannot be found.
        /// </summary>
        GeneralFailure = -1,
        /// <summary>
        /// A system status or system Help reply.
        /// </summary>
        SystemStatus = 211,
        /// <summary>
        /// A Help message was returned by the service.
        /// </summary>
        HelpMessage = 214,
        /// <summary>
        /// The SMTP service is ready.
        /// </summary>
        ServiceReady = 220,
        /// <summary>
        /// The SMTP service is closing the transmission channel.
        /// </summary>
        ServiceClosingTransmissionChannel = 221,
        /// <summary>
        /// The email was successfully sent to the SMTP service.
        /// </summary>
        Ok = 250,
        /// <summary>
        /// The user mailbox is not located on the receiving server; the server forwards the e-mail.
        /// </summary>
        UserNotLocalWillForward = 251,
        /// <summary>
        /// The client successfully complete the authentication exchange
        /// </summary>
        AuthenticationCompleted = 235,
        /// <summary>
        /// The specified user is not local, but the receiving SMTP service accepted the message and attempted to deliver it. This status code is defined in RFC 1123, which is available at http://www.ietf.org.
        /// </summary>
        CannotVerifyUserWillAttemptDelivery = 252,
        /// <summary>
        /// The SMTP service is ready to start authentication.
        /// </summary>
        StartAuthenticationInput = 334,
        /// <summary>
        /// The SMTP service is ready to receive the e-mail content.
        /// </summary>
        StartMailInput = 354,
        /// <summary>
        /// The SMTP service is not available; the server is closing the transmission channel.
        /// </summary>
        ServiceNotAvailable = 421,
        /// <summary>
        /// The destination mailbox is in use.
        /// </summary>
        MailboxBusy = 450,
        /// <summary>
        /// The SMTP service cannot complete the request. This error can occur if the client's IP address cannot be resolved (that is, a reverse lookup failed).
        /// You can also receive this error if the client domain has been identified as an open relay or source for unsolicited e-mail (spam). 
        /// For details, see RFC 2505, which is available at http://www.ietf.org.
        /// </summary>
        LocalErrorInProcessing = 451,
        /// <summary>
        /// The SMTP service does not have sufficient storage to complete the request.
        /// </summary>
        InsufficientStorage = 452,
        /// <summary>
        /// The client was not authenticated or is not allowed to send mail using the specified SMTP host.
        /// </summary>
        ClientNotPermitted = 454,
        /// <summary>
        /// The SMTP service does not recognize the specified command.
        /// </summary>
        CommandUnrecognized = 500,
        /// <summary>
        /// The syntax used to specify a command or parameter is incorrect.
        /// </summary>
        SyntaxError = 501,
        /// <summary>
        /// The SMTP service does not implement the specified command.
        /// </summary>
        CommandNotImplemented = 502,
        /// <summary>
        /// The commands were sent in the incorrect sequence.
        /// </summary>
        BadCommandSequence = 503,
        /// <summary>
        /// The SMTP service does not implement the specified command parameter.
        /// </summary>
        CommandParameterNotImplemented = 504,
        /// <summary>
        /// The SMTP server is configured to accept only TLS connections, and the SMTP client is attempting to connect by using a non-TLS connection. 
        /// The solution is for the user to set EnableSsl=true on the SMTP 22.
        /// </summary>
        MustIssueStartTlsFirst = 530,
        /// <summary>
        /// SMTP Authentication unsuccessful/Bad username or password
        /// </summary>
        AuthenticationFailed = 535,
        /// <summary>
        /// The destination mailbox was not found or could not be accessed.
        /// </summary>
        MailboxUnavailable = 550,
        /// <summary>
        /// The user mailbox is not located on the receiving server. You should resend using the supplied address information.
        /// </summary>
        UserNotLocalTryAlternatePath = 551,
        /// <summary>
        /// The message is too large to be stored in the destination mailbox.
        /// </summary>
        ExceededStorageAllocation = 552,
        /// <summary>
        /// The syntax used to specify the destination mailbox is incorrect.
        /// </summary>
        MailboxNameNotAllowed = 553,
        /// <summary>
        /// The transaction failed.
        /// </summary>
        TransactionFailed = 554,
    }
}
