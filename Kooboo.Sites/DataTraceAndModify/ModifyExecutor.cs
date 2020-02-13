using Kooboo.Data.Context;
using Kooboo.Sites.DataTraceAndModify.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify
{
    public static class ModifyExecutor
    {
        static ModifierBase[] _modifiers = new[]{
            new TextContentModifier()
        };


        public static void Execute(RenderContext renderContext, List<ModifierBase> changedList)
        {
            foreach (var item in changedList)
            {
                var modifier = _modifiers.First(f => f.Source == item.Source);
                modifier.Modify(renderContext);
            }
        }

        public static Type GetModifierType(string source)
        {
            return _modifiers.First(f => f.Source == source).GetType();
        }
    }
}
