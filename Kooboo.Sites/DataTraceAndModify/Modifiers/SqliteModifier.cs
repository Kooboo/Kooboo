using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class SqliteModifier : ModifierBase
    {
        public override string Source => "sqlite";

        public override void Modify(RenderContext context)
        {
            throw new NotImplementedException();
        }
    }
}
