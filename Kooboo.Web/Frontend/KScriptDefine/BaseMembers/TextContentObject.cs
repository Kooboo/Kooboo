using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface TextContentObject
    {
        [Description("set to read content properties based on a different culture")]
        TextContentObject SetCulture(string culture);
    }
}
