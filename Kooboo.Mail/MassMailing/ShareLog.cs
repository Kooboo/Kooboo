using System;

namespace Kooboo.Mail.MassMailing
{
    public class ShareLog
    {
        public string EmailAddress { get; set; }

        public bool Success { get; set; }

        public string Log { get; set; }

        public DateTime SentTime { get; set; }

        public string SendTaskId { get; set; }

        public string RecipientId { get; set; }

        public string WebSiteId { get; set; }
    }

}
