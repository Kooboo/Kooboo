using Kooboo.Data.Interface;

namespace Kooboo.Sites.Scripting.Global.SMS
{
    public class ChinaMobileSMSSetting : ISiteSetting
    {
        public string Name => "ChinaMobileSMSSetting";

        public string ecName { get; set; }

        public string apId{ get; set; }

        public string secretKey { get; set; }

        public string sign { get; set; }
    }
}
