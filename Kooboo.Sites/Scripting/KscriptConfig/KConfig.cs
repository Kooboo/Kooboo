using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.KscriptConfig
{
    public class KConfig
    {
        public string ExtensionDlls { get; set; }

        public Dictionary<string, string> Kscripts { get; set; } = new Dictionary<string, string>();
    }
}
