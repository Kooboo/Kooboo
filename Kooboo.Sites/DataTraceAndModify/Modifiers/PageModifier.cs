using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    class PageModifier : ModifierBase
    {
        public override string Source => "page";

        public override void Modify(RenderContext context)
        {
            HandleDom(context);
        }
    }
}
