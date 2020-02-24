using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class LayoutModifier : ModifierBase
    {
        public override string Source => "layout";

        public override void Modify(RenderContext context)
        {
            UpdateSiteObject(context);
        }
    }
}
