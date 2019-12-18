using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface Response
    {
        [Description("Excute another Url, and write the response within current context")]
        void Execute(string url);

        [Description("Print the object in Json format, if the object is a value type like string, or number, it will print the string format of that value.")]
        void json(object value);

        [Description("Redirect user to another url, url can be relative or absolute, status code 302")]
        void redirect(string url);

        [Description("set header value on output html page.")]
        void setHeader(string key, string value);

        [Description("Print the input on output page. If it is not a value type object, it will print Json format of that object.")]
        void write(object value);
    }
}
