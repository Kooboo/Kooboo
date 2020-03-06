using Kooboo.Data.Context;
using KScript;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class KeyValueModifier : ModifierBase
    {
        public override string Source => "keyvalue";
        public string Key => GetValue("key");

        public override void Modify(RenderContext context)
        {
            if (string.IsNullOrEmpty(Key)) return;
            var kInstance = new k(context);

            if (kInstance.KeyValue.ContainsKey(Key))
            {
                kInstance.KeyValue.set(Key, Value);
            }
        }
    }
}
