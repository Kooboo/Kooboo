using Kooboo.Data.Context;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Scripting.Global.SMS
{
    public class ChinaMobileSMSSetting : ISiteSetting, ISettingDescription
    {
        public string Name => "ChinaMobileSMSSetting";

        public string ecName { get; set; }

        public string apId { get; set; }

        public string secretKey { get; set; }

        public string sign { get; set; }

        public string Group => "SMS";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
