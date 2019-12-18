using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface RoutableTextRepository : TextRepository
    {
        [Description("get the absolute Url of this object")]
        SiteObject GetAbsUrl(object id);

        [Description("get the SiteObject by Url")]
        SiteObject GetByUrl(string url);

        [Description("get the relative Url of this object")]
        SiteObject GetUrl(object id);
    }
}
