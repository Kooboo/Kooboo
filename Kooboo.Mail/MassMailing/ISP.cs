using System.Collections.Generic;

namespace Kooboo.Mail.MassMailing.Model
{

    public class ISP
    {
        public string Name { get; set; }

        public bool StartTls { get; set; }

        public bool PipeLining { get; set; }

        public HashSet<string> SendingCountry { get; set; }   // This ISP can only be sent from certain country...Received Mails will forward to this country. 

        public HashSet<string> RootDomains { get; set; }


        private HashSet<string> _mxDomains;
        public HashSet<string> MxDomains
        {
            get
            {
                if (_mxDomains == null)
                {
                    _mxDomains = new HashSet<string>();
                }
                return _mxDomains;
            }
            set
            {
                _mxDomains = value;
            }
        }

        private IPISPQuota _quota;
        public IPISPQuota Quota
        {
            get
            {
                if (_quota == null)
                {
                    _quota = new IPISPQuota();
                }
                return _quota;
            }
            set
            {
                _quota = value;
            }

        }

    }
}
