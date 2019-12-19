using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface Url
    {
        [Description("Get data string from the url")]
        string Get(string url);

        [Description("get data string from remote url by using HTTP Basic authentication")]
        string Get(string url, string username, string password);

        [Description("Post data to remote url")]
        string Post(string url, string data);

        [Description("Post data to remote url using HTTP basic authentication")]
        string Post(string url, string data, string userName, string password);

        [Description("Post data as JSON to remote url using HTTP basic authentication")]
        string PostData(string url, object data, string userName, string password);
    }
}
