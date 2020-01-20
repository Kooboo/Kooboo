using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System;
using System.Collections.Generic;

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

        public override string[] DataTypes => new string[] { "NULL", "INTEGER", "REAL", "TEXT", "BLOB" };

        internal override string ConventType(Type type)
        {
            if (type == typeof(string)) return "TEXT";
            if (type == typeof(double) || type == typeof(int) || type == typeof(float) || type == typeof(decimal)) return "REAL";
            if (type == null) return "NULL";
            if (type == typeof(bool)) return "INTEGER";
            throw new NotSupportedException();
        }
    }
}
