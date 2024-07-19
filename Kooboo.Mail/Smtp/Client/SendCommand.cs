namespace Kooboo.Mail.Smtp.Client
{
    public class SendCommand
    {
        public string CommandLine { get; set; }

        public string Command { get; set; }   // used for logging. 

        public SmtpStatusCode ExpectedResponse { get; set; }
    }
}
