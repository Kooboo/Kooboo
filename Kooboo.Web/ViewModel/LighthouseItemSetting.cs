using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.ViewModel
{
    public class LighthouseItemSetting
    {
        public string Name { get; set; }
        public bool Enable { get; set; }

        public InnerSetting[] Setting { get; set; }

        public class InnerSetting
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }
    }
}
