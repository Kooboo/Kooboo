using Kooboo.Web.Frontend.KScriptDefine.BaseMembers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine
{
    public interface KScript
    {
        [Description("Access to the http request data, query string, form or headers. Cookie is available from k.cookie.")]
        Request Request { get; set; }

        [Description("The http response object that is used to set data into http resposne stream")]
        Response Response { get; set; }

        [Description("a temporary storage for small interactive information. Session does not persist")]
        Session Session { get; set; }

        [Description("Get or set cookie value")]
        Cookie Cookie { get; set; }

        [Description("The Kooboo website database with version control")]
        Site Site { get; set; }

        [Description("Get content from or post data to remote url.")]
        Url Url { get; set; }
    }
}
