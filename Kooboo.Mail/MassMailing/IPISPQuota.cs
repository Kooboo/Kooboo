namespace Kooboo.Mail.MassMailing.Model
{
    public class IPISPQuota
    {
        public string LocalIP { get; set; }

        public string ISPName { get; set; }

        public int MaxConnections { get; set; } = 1;

        public int MinutelyQuota { get; set; } = 60;

        public int HourlyQuota { get; set; } = 60 * 60;

        public int DailyQuota { get; set; } = 60 * 60 * 24;

        public int MailsPerConnection { get; set; } = 10;
    }
}