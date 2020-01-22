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

        static readonly string[] _textType = new[] { "CHAR", "VARCHAR", "TINYBLOB", "TINYTEXT", "BLOB", "TEXT", "MEDIUMBLOB", "MEDIUMTEXT", "LONGBLOB", "LONGTEXT"};
        static readonly string[] _dateTime = new[] { "DATE", "TIME", "YEAR", "DATETIME" };
        static readonly string[] _double = new[] { "FLOAT", "DOUBLE", "DECIMAL", "BIGINT", "INTEGER", "INT", "MEDIUMINT", "SMALLINT", "YEAR","TINYINT" };

        internal override string ConventType(Type type)
        {
            if (type == typeof(string)) return "TEXT";
            if (type == typeof(DateTime)) return "DATETIME";
            if (type == typeof(double) || type == typeof(int) || type == typeof(float) || type == typeof(decimal)) return "DOUBLE";
            if (type == null) return "NULL";
            if (type == typeof(bool)) return "TINYINT";
            throw new NotSupportedException();
        }

        internal override bool CompatibleType(string dbType, string jsType)
        {
            if (jsType == "NULL") return true;
            if (jsType == "TEXT" && _textType.Any(a => dbType.ToUpper().StartsWith(a))) return true;
            if (jsType == "DATETIME" && _dateTime.Any(a => dbType.ToUpper().StartsWith(a))) return true;
            if (jsType == "DOUBLE" && _double.Any(a => dbType.ToUpper().StartsWith(a))) return true;
            if (jsType == "TINYINT" && _double.Any(a => dbType.ToUpper().StartsWith(a))) return true;
            return false;
        }
    }
}
