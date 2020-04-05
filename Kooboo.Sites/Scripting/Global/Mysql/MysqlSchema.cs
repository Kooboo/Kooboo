using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        static readonly string[] _double = new[] { "FLOAT", "DOUBLE", "DECIMAL", "BIGINT", "INTEGER", "INT", "MEDIUMINT", "SMALLINT", "YEAR", "TINYINT" };

        internal override string ConventType(Type type)
        {
            if (type == typeof(string) || type == typeof(Regex)) return "VARCHAR";
            if (type == typeof(DateTime)) return "DATETIME";
            if (type == typeof(double) || type == typeof(int) || type == typeof(float) || type == typeof(decimal)) return "DOUBLE";
            if (type == null) return "NULL";
            if (type == typeof(bool)) return "TINYINT";
            return "VARCHAR";
        }
    }
}
