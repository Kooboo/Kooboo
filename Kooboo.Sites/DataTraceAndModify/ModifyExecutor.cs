using Jint.Native.Number.Dtoa;
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
        static readonly Lazy<ModifierBase[]> _modifiers = new Lazy<ModifierBase[]>(() =>
        {
            bool basedModifierBase(Type type)
            {
                if (type == null) return false;
                else if (type == typeof(ModifierBase)) return true;
                else
                {
                    return basedModifierBase(type.BaseType);
                }
            }

            return Lib.Reflection.AssemblyLoader.AllAssemblies
            .SelectMany(s => s.GetTypes())
            .Where(s => !s.IsAbstract && basedModifierBase(s))
            .Select(s => (ModifierBase)Activator.CreateInstance(s))
            .ToArray();
        }, true);


        public static void Execute(RenderContext renderContext, List<ModifierBase> changedList)
        {
            foreach (var item in changedList)
            {
                item.Modify(renderContext);
            }
        }

        public static Type GetModifierType(string source)
        {
            var modifier = _modifiers.Value.FirstOrDefault(f => f.Source == source);
            if (modifier == null) throw new NotImplementedException();
            return modifier.GetType();
        }
    }
}
