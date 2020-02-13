using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify
{
    public abstract class ModifierBase
    {

        public abstract string Source { get; }

        public abstract void Modify(RenderContext context);
    }
}
