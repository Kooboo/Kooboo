using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.SMS
{
  public  class AliSMSSetting : ISiteSetting
    {
        public string accessId { get; set; }
        public string accessSecret { get; set; }

        public string regionId { get; set; }

        public string signName { get; set; }

        public string Name => "AliSMS";
    }
      


}
