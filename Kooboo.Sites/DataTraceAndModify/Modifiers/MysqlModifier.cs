using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class MysqlModifier : ModifierBase
    {
        public override string Source => "mysql";

        public override void Modify(RenderContext context)
        {
            throw new NotImplementedException();
        }
    }
}
