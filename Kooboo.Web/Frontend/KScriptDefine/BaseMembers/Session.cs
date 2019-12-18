using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface Session
    {
        object[] Keys { get; set; }
        object[] Values { get; set; }

        [Description("remove all items from session")]
        void Clear();

        [Description("check whether session has the key or not.")]
        bool Contains(string key);

        [Description("get stored session value")]
        object Get(string key);

        [Description("Remove item from session by session key.")]
        void Remove(string key);

        [Description("Set a Key Value in the session store.")]
        void Set(string key, object value);
    }
}
