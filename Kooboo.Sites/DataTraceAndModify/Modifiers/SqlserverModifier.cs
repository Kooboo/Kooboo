using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class SqlserverModifier : ModifierBase
    {
        public override string Source => "sqlserver";

        public override void Modify(RenderContext context)
        {
            throw new NotImplementedException();
        }
    }
}
