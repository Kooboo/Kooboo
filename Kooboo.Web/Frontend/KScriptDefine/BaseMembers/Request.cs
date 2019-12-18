using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface Request
    {
        [Description("The query string collection")]
        Dictionary QueryString { get; set; }
        Dictionary Form { get; set; }
        string Method { get; set; }
        string ClientIp { get; set; }
        Dictionary Headers { get; set; }
        string Url { get; set; }
        UploadFile[] Files { get; set; }
    }
}
