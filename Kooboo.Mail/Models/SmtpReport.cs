using System;

namespace Kooboo.Mail.Models
{
    public class SmtpReport
    {

        public static Guid ToGuid(string messageid)
        {
            if (messageid == null)
            {
                return Guid.Empty;
            }

            var index = messageid.IndexOf("<");
            if (index > -1)
            {
                var lastindex = messageid.LastIndexOf(">");
                if (lastindex > -1)
                {
                    messageid = messageid.Substring(index + 1, lastindex - index - 1);
                }
            }

            return Lib.Security.Hash.ComputeGuidIgnoreCase(messageid);
        }

        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = ToGuid(this.MessageId);
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public bool IsSuccess { get; set; }
        public string Log { get; set; }
        public string MessageId { get; set; }
        public string HeaderFrom { get; set; }
        public string RcptTo { get; set; }
        public long DateTick { get; set; } // utc...
        public string Subject { get; set; }
        public string Json { get; set; }   // This is every single delivery.
    }

    public record SmtpReportHeaders
    {
        public string Date { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }
        public string HeaderFrom { get; set; }
    }

    public record SmtpReportIn
    {
        public bool IsSuccess { get; set; }
        public string Log { get; set; }
        public string MessageId { get; set; }
        public string HeaderFrom { get; set; }
        public string RcptTo { get; set; }
        public long DateTick { get; set; }

        public string Subject { get; set; }

    }
}
