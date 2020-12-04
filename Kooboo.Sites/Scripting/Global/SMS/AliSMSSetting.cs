using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.SMS
{
  public  class AliSMSSetting : ISiteSetting,ISettingDescription
    {
        public string accessId { get; set; }
        public string accessSecret { get; set; }

        public string regionId { get; set; }

        public string signName { get; set; }
        public string internationalSignName { get; set; }

        public string Name => "AliSMS";

        public string Group => "SMS";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
      


}
