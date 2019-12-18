using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface Dictionary
    {
        object Keys { get; set; }
        object Values { get; set; }
        int Length { get; set; }
        string Item { get; set; }

        [Description("add new value into the collection.")]
        void Add(string key, string value);
        [Description("check whether the collection has that key or not.")]
        bool Contains(object key);
        [Description("get value form the collection.")]
        string Get(string key);
    }
}
