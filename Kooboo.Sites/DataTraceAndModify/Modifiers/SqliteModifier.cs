using Kooboo.Data.Context;
using KScript;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class SqliteModifier : DataBaseModifier
    {
        public override string Source => "sqlite";

        internal override ITable GetTable(k kInstance)
        {
            return kInstance.Sqlite.GetTable(Table);
        }
    }
}
