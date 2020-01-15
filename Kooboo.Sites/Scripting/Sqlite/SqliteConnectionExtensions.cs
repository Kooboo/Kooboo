using Dapper;
using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Sqlite
{
    public static class SqliteConnectionExtensions
    {
        public static SqliteSchema GetSchema(this SQLiteConnection sqliteConnection, string name)
        {
            var items = sqliteConnection.Query<SqliteSchema.Item>($"PRAGMA  table_info('{name}');");
            return new SqliteSchema(items);
        }

        public static void UpgradeSchema(this SQLiteConnection sqliteConnection, string name, IEnumerable<SqliteSchema.Item> items)
        {
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                sb.AppendLine($"ALTER TABLE {name} ADD COLUMN {item.Name} {item.Type.ToString()};");
            }

            sqliteConnection.Execute(sb.ToString());
        }

        public static void CreateTable(this SQLiteConnection sqliteConnection, string name)
        {
            sqliteConnection.Execute($"CREATE TABLE {name} ( _id TEXT PRIMARY KEY)");
        }

    }
}
