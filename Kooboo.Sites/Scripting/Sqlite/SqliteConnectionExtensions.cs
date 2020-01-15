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

        public static void Insert(this SQLiteConnection sqliteConnection, string name, object data)
        {
            var dic = data as IDictionary<string, object>;
            var columns = string.Join(",", dic.Select(s => s.Key));
            var values = string.Join(",", dic.Select(s => $"@{s.Key}"));
            sqliteConnection.Execute($"INSERT INTO {name} ({columns}) VALUES ({values})", data);
        }

        public static void Append(this SQLiteConnection sqliteConnection, string name, object data, SqliteSchema schema)
        {
            var dic = data as IDictionary<string, object>;
            var removeKeys = new List<string>();

            foreach (var item in dic)
            {
                if (schema.Items.All(a => a.Name != item.Key))
                {
                    removeKeys.Add(item.Key);
                }
            }

            foreach (var item in removeKeys)
            {
                dic.Remove(item);
            }

            var columns = string.Join(",", dic.Select(s => s.Key));
            var values = string.Join(",", dic.Select(s => $"@{s.Key}"));
            sqliteConnection.Execute($"INSERT INTO {name} ({columns}) VALUES ({values})", data);
        }

        public static void CreateIndex(this SQLiteConnection sqliteConnection, string name, string fieldname)
        {
            sqliteConnection.Execute($"CREATE INDEX {fieldname} on {name}({fieldname})");
        }

        public static void Delete(this SQLiteConnection sqliteConnection, string name, string id)
        {
            sqliteConnection.Execute($"DELETE FROM {name} WHERE _id = @Id", new { Id = id });
        }

        public static object[] All(this SQLiteConnection sqliteConnection, string name)
        {
            return sqliteConnection.Query<object>($"SELECT * FROM {name}").ToArray();
        }

        public static void UpdateData(this SQLiteConnection sqliteConnection, string name, string id, object data)
        {
            var dic = data as IDictionary<string, object>;
            var keyValues = string.Join(",", dic.Select(s => $"{s.Key}=@{s.Key}"));
            sqliteConnection.Execute($"UPDATE {name} SET {keyValues} WHERE _id = @Id", data);
        }

        public static object Get(this SQLiteConnection sqliteConnection, string name, string id)
        {
           return sqliteConnection.Query<object>($"SELECT * FROM {name} WHERE _id = @Id", new { Id = id }).FirstOrDefault();
        }
    }
}
