using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class SqlServerSchema : RelationalSchema
    {
        public SqlServerSchema(IEnumerable<Item> items) : base(items)
        {
        }

        public SqlServerSchema(IDictionary<string, object> keyValuePairs) : base(keyValuePairs)
        {
        }

        internal override string ConventType(Type type)
        {
            if (type == typeof(string)) return "TEXT";
            if (type == typeof(DateTime)) return "DATETIME";
            if (type == typeof(double) || type == typeof(int) || type == typeof(float) || type == typeof(decimal)) return "NUMERIC";
            if (type == null) return "NULL";
            if (type == typeof(bool)) return "BIT";
            throw new NotSupportedException();
        }
    }
}
