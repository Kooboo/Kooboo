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
        static  List<ModifierBase> _modifiers = new List<ModifierBase>{
            new TextContentModifier(),
            new ViewModifier()
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
            var modifier = _modifiers.FirstOrDefault(f => f.Source == source);
            if (modifier == null) throw new NotImplementedException();
            return modifier.GetType();
        }
    }
}
