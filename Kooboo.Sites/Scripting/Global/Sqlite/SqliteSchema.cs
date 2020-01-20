using Kooboo.Sites.Scripting.Global.Database;
using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (type == typeof(string) || type == typeof(DateTime)) return "TEXT";
            if (type == typeof(double) || type == typeof(int) || type == typeof(float) || type == typeof(decimal)) return "REAL";
            if (type == null) return "NULL";
            if (type == typeof(bool)) return "INTEGER";
            throw new NotSupportedException();
        }

        internal override string StandardizationType(string type)
        {
            switch (type)
            {
                case "NULL":
                    return "NULL";
                case "INTEGER":
                    return "INTEGER";
                case "REAL":
                    return "REAL";
                case "TEXT":
                case "BLOB":
                    return "TEXT";
                default:
                    return "NULL";
            }
        }
    }
}
