using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class MysqlSchema : RelationalSchema
    {
        public MysqlSchema(IEnumerable<Item> items) : base(items)
        {
        }

        public MysqlSchema(IDictionary<string, object> keyValuePairs) : base(keyValuePairs)
        {
        }

        public override string[] DataTypes => new[] { "TINYINT", "SMALLINT", "MEDIUMINT", "INT", "BIGINT" };

        internal override string ConventType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
