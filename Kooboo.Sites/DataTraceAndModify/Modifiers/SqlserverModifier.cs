using Kooboo.Data.Context;
using KScript;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class SqlserverModifier : DataBaseModifier
    {
        public override string Source => "sqlserver";

        internal override ITable GetTable(k kInstance)
        {
            return kInstance.SqlServer.GetTable(Table);
        }
    }
}
