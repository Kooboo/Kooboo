using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        static readonly string[] _textType = new[] { "CHAR", "VARCHAR", "TINYBLOB", "TINYTEXT", "BLOB", "TEXT", "MEDIUMBLOB", "MEDIUMTEXT", "LONGBLOB", "LONGTEXT" };
        static readonly string[] _dateTime = new[] { "DATE", "TIME", "YEAR", "DATETIME" };
        static readonly string[] _double = new[] { "FLOAT", "DOUBLE", "DECIMAL", "BIGINT", "INTEGER", "INT", "MEDIUMINT", "SMALLINT" };

        internal override string ConventType(Type type)
        {
            if (type == typeof(string)) return "TEXT";
            if (type == typeof(DateTime)) return "DATETIME";
            if (type == typeof(double) || type == typeof(int) || type == typeof(float) || type == typeof(decimal)) return "DOUBLE";
            if (type == null) return "NULL";
            if (type == typeof(bool)) return "TINYINT";
            throw new NotSupportedException();
        }

        internal override string StandardizationType(string type)
        {
            if (type.ToUpper() == "NULL") return "NULL";
            if (_textType.Any(a => type.ToUpper().StartsWith(a))) return "TEXT";
            if (_dateTime.Any(a => type.ToUpper().StartsWith(a))) return "DATETIME";
            if (_double.Any(a => type.ToUpper().StartsWith(a))) return "DOUBLE";
            return "NULL";
        }
    }
}
