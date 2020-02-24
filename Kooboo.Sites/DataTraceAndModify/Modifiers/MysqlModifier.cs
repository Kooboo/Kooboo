using Kooboo.Data.Context;
using KScript;
using System;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class MysqlModifier : DataBaseModifier
    {
        public override string Source => "mysql";

        internal override ITable GetTable(k kInstance)
        {
            return kInstance.Mysql.GetTable(Table);
        }
    }
}
