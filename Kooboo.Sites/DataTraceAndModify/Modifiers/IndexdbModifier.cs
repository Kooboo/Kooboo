using KScript;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class IndexdbModifier : DataBaseModifier
    {
        public override string Source => "indexdb";

        internal override ITable GetTable(k kInstance)
        {
            return kInstance.Database.GetTable(Table);
        }
    }
}
