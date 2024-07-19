using System;

namespace Kooboo.Mail.Models
{

    public class DmarcReport
    {
        public long Id { get; set; } // This is auto incremental
        public string ReportDomain { get; set; }  //eg.google.com, microsoft.com.

        public string OurDomain { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool Pass
        {
            get
            {
                if (Spf == "pass" && Dkim == "pass")
                {
                    return true;
                }
                return false;
            }
        }

        public string Spf { get; set; }

        public string Dkim { get; set; }

        public int Count { get; set; }

        public string SourceIP { get; set; }

        public string MailFrom { get; set; }

        public string RcptTo { get; set; }

        public string MsgFrom { get; set; }
    }


}
