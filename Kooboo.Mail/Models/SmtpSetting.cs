namespace Kooboo.Mail.Models
{

    public class SmtpSetting
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public bool SSL { get; set; }

        public string From { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
