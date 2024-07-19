namespace Kooboo.Mail.Events
{
    public class ArgBase
    {
        public Message Message { get; set; }

        public EnumMailEvent Event { get; set; }

        public MailDb UserMail { get; set; }

    }

}
