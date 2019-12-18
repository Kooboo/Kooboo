using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface Cookie
    {
        object[] Keys { get; set; }
        object[] Values { get; set; }
        int Length { get; set; }
        string Item { get; set; }

        [Description("remove all items from cookie")]
        void Clear();

        [Description("check whether cookie has the key or not.")]
        bool ContainsKey(string key);

        [Description("Get the cookie value by name")]
        string Get(string name);

        [Description("Remove item from cookie by session key.")]
        void Remove(string key);

        [Description("set a cookie with defined expiration days")]
        void Set(string name, string value, int days);

        [Description("set a cookie that expires in 1 day.")]
        void Set(string name, string value);

        [Description("set the cookie with an expiration time in minutes.")]
        void SetByMinutes(string name, string value, int mins);
    }
}
