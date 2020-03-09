using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class MenuModifier : DomModifier
    {
        public override string Source => "menu";

        public override void Modify(RenderContext context)
        {
            //current not menu dom modify demand
        }
    }
}
