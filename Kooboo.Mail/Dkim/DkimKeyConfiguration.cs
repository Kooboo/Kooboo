using System;
using System.Collections.Generic;

namespace Kooboo.Mail.Dkim
{
    public class DkimKeyConfiguration
    {
        private DkimKeyConfiguration() { }

        public DkimKeyConfiguration(Func<Dictionary<string, RsaKey>> GetDkimKey)
        {
            Keys = GetDkimKey;
        }

        public Func<Dictionary<string, RsaKey>> Keys { get; set; } = null!;

    }
}

