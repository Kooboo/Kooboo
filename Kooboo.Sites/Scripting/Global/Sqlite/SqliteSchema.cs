using Kooboo.Sites.Scripting.Global.Database;
using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KScript
{
    public class SqliteSchema : RelationalSchema
    {
        public SqliteSchema(IDictionary<string, object> keyValuePairs) : base(keyValuePairs)
        {
        }

        public SqliteSchema(IEnumerable<Item> items) : base(items)
        {
           
        }

        internal override string ConventType(Type type)
        {
            if (type == typeof(string) || type == typeof(DateTime) || type == typeof(Regex)) return "TEXT";
            if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return "REAL";
            if (type == null) return "NULL";
            if (type == typeof(bool) || type == typeof(int) || type == typeof(long)) return "INTEGER";
            return "TEXT";
        }
    }
}
