using System;
using System.Collections.Generic;
using Kooboo.Dom;
using Kooboo.Infrastructure.IoC.Discovery;
using Kooboo.Sites.Render;

namespace Kooboo.TAL.Binding
{
    public abstract class TalBinder
    {
        public abstract string AttributeName { get; }

        public abstract Node ApplyBinding(Element element, string attributeValue, FrontContext context);

        static readonly Dictionary<string, TalBinder> Binders = new Dictionary<string,TalBinder>();

        static TalBinder()
        {
            foreach (var asm in WebAppAssemblySource.Current.GetAssemblies())
            {
                foreach (var type in asm.GetExportedTypes())
                {
                    if (type.IsClass && !type.IsAbstract && typeof (TalBinder).IsAssignableFrom(type))
                    {
                        var binder = (TalBinder) Activator.CreateInstance(type);
                        Binders.Add(binder.AttributeName, binder);
                    }
                }
            }
        }

    
    }
}
