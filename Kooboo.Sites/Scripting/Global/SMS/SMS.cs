using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.SMS
{
  public  class SMS
    {
        public RenderContext Context { get; set; }

        public SMS(RenderContext context)
        {
            this.Context = context; 
        }

        [Description("Send SMS using AliCloud")]
        public Ali AliSMS
        {
            get
            {
                return new Ali(this.Context); 
            }
        }
        [Description("Send SMS using TencentCloud")]
        public Tencent Tencent
        {
            get
            {
                return new Tencent(this.Context); 
            }
        }
        [Description("Send SMS using ChinaMobile")]
        public ChinaMobile ChinaMobile
        {
            get
            {
                return new ChinaMobile(this.Context);
            }
        }
    }
}
