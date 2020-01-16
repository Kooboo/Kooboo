using Dapper;
using Jint.Parser.Ast;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.IndexedDB.Query;
using Kooboo.Sites.Scripting.Global.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace KScript
{
    public static class SqliteConnectionExtensions
    {
        public static SqliteSchema GetSchema(this SQLiteConnection sqliteConnection, string name)
        {
            var items = sqliteConnection.Query<SqliteSchema.Item>($@"SELECT ""name"",""type"" as _type FROM pragma_table_info ('{name}');");
            return new SqliteSchema(items);
        }

        public static void UpgradeSchema(this SQLiteConnection sqliteConnection, string name, IEnumerable<SqliteSchema.Item> items)
        {
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                sb.AppendLine($@"ALTER TABLE ""{name}"" ADD COLUMN {item.Name} {item.Type.ToString()};");
            }

            sqliteConnection.Execute(sb.ToString());
        }

        public static void CreateTable(this SQLiteConnection sqliteConnection, string name)
        {
            sqliteConnection.Execute($@"CREATE TABLE ""{name}"" ( _id TEXT PRIMARY KEY)");
        }

        public static void Insert(this SQLiteConnection sqliteConnection, string name, object data)
        {
            var dic = data as IDictionary<string, object>;
            var columns = string.Join(",", dic.Select(s => $@"""{s.Key}"""));
            var values = string.Join(",", dic.Select(s => $"@{s.Key}"));
            sqliteConnection.Execute($@"INSERT INTO ""{name}""({columns}) VALUES ({values})", new[] { data });
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

            var columns = string.Join(",", dic.Select(s => $@"""{s.Key}"""));
            var values = string.Join(",", dic.Select(s => $"@{s.Key}"));
            sqliteConnection.Execute($@"INSERT INTO ""{name}"" ({columns}) VALUES ({values})", data);
        }

        public static void CreateIndex(this SQLiteConnection sqliteConnection, string name, string fieldname)
        {
            sqliteConnection.Execute($@"CREATE INDEX {fieldname} on ""{name}""(""{fieldname}"")");
        }

        public static void Delete(this SQLiteConnection sqliteConnection, string name, string id)
        {
            sqliteConnection.Execute($@"DELETE FROM ""{name}"" WHERE _id = @Id", new { Id = id });
        }

        public static void UpdateData(this SQLiteConnection sqliteConnection, string name, string id, object data)
        {
            var dic = data as IDictionary<string, object>;
            var keyValues = string.Join(",", dic.Select(s => $@"""{s.Key}""=@{s.Key}"));
            sqliteConnection.Execute($@"UPDATE ""{name}"" SET {keyValues} WHERE _id = @Id", data);
        }

        public static RelationModel GetRelation(this SQLiteConnection sqliteConnection, string name, string relation)
        {
            var sql = $@"SELECT ""table"",""from"",""to"" FROM pragma_foreign_key_list('{name}') where ""table""='{relation}';";
            var relations = sqliteConnection.Query<RelationModel>(sql);
            return relations.FirstOrDefault();
        }

        public static object[] QueryData(this SQLiteConnection sqliteConnection, string name, string where = null, long? limit = null, long? offset = null)
        {
            var conditions = QueryPraser.ParseConditoin(where);
            var whereStr = where == null ? string.Empty : $"WHERE {ConditionsToSql(conditions)}";
            var limitStr = limit.HasValue ? $"LIMIT {limit}" : string.Empty;
            var offsetStr = offset.HasValue && offset != 0 ? $"OFFSET {offset}" : string.Empty;
            return sqliteConnection.Query<object>($@"SELECT * FROM ""{name}"" {whereStr} {limitStr} {offsetStr}").ToArray();
        }

        public static int Count(this SQLiteConnection sqliteConnection, string name, string where = null, long? limit = null, long? offset = null)
        {
            var conditions = QueryPraser.ParseConditoin(where);
            var whereStr = where == null ? string.Empty : $"WHERE {ConditionsToSql(conditions)}";
            var limitStr = limit.HasValue ? $"LIMIT {limit}" : string.Empty;
            var offsetStr = offset.HasValue && offset != 0 ? $"OFFSET {offset}" : string.Empty;
            return sqliteConnection.Query<int>($@"SELECT count(*) FROM ""{name}"" {whereStr} {limitStr} {offsetStr}").FirstOrDefault();
        }

        private static string ConditionsToSql(List<ConditionItem> conditions)
        {
            return string.Join(" and ", conditions.Select(s => $@" ""{s.Field}"" {ComparerToString(s.Comparer)} {ConventValue(s.Comparer, s.Value)} "));
        }

        static string ComparerToString(Comparer comparer)
        {
            switch (comparer)
            {
                case Comparer.EqualTo:
                    return "=";
                case Comparer.GreaterThan:
                    return ">";
                case Comparer.GreaterThanOrEqual:
                    return ">=";
                case Comparer.LessThan:
                    return "<";
                case Comparer.LessThanOrEqual:
                    return "<=";
                case Comparer.NotEqualTo:
                    return "<>";
                case Comparer.StartWith:
                    return "like";
                case Comparer.Contains:
                    return "like";
                default:
                    throw new NotSupportedException();
            }
        }

        static string ConventValue(Comparer comparer, string value)
        {
            switch (comparer)
            {
                case Comparer.EqualTo:
                case Comparer.NotEqualTo:

                    if (!decimal.TryParse(value, out var _) && !bool.TryParse(value, out var _))
                    {
                        value = $"'{value}'";
                    }

                    break;
                case Comparer.StartWith:
                    value = $"'{value}%'";
                    break;
                case Comparer.Contains:
                    value = $"'%{value}%'";
                    break;
                default:
                    break;
            }

            return value;
        }
    }
}
