using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface MultilingualObject
    {
        [Description("Get or set the value of current culture")]
        string Value { get; set; }
    }
}
