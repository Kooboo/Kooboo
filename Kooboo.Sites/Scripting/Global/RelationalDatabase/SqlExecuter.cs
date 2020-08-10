using Dapper;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.IndexedDB.Query;
using Kooboo.Sites.Scripting.Interfaces;
using KScript;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public abstract class SqlExecuter<T> : ISqlExecuter
        where T : IDbConnection
    {
        readonly string _connectionString;

        public abstract char QuotationLeft { get; }
        public abstract char QuotationRight { get; }
        public virtual char StatementDelimiter => ';';

        protected SqlExecuter(string connectionSring)
        {
            _connectionString = connectionSring;
        }

        public IDbConnection CreateConnection() => (T)Activator.CreateInstance(typeof(T), _connectionString);

        public abstract RelationalSchema GetSchema(string name);

        public virtual void UpgradeSchema(string name, IEnumerable<RelationalSchema.Item> items)
        {
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                var type = item.Type;
                var length = item.Length > 0 && type.ToLower() != "text" ? $"({item.Length})" : string.Empty;
                sb.AppendLine($@"ALTER TABLE {WarpField(name)} ADD COLUMN {WarpField(item.Name)} {type}{length};");
            }

            using (var connection = CreateConnection())
            {
                connection.Execute(sb.ToString());
            }
        }

        public abstract void CreateTable(string name);

        public virtual object Insert(string name, object data, RelationalSchema schema, bool returnId = false)
        {
            var dic = data as IDictionary<string, object>;
            var columns = string.Join(",", dic.Select(s => $@"{WarpField(s.Key)}"));
            var values = string.Join(",", dic.Select(s => $"@{s.Key}"));
            var sql = $@"INSERT INTO {WarpField(name)}({columns}) VALUES ({values})";

            if (!returnId)
            {
                using (var connection = CreateConnection())
                {
                    connection.Execute(sql, new[] { data });
                    return null;
                }
            }

            var whereCaluse = String.Join(" and ", dic.Select(o => $"{WarpField(o.Key)}=@{o.Key}"));
            sql += StatementDelimiter + $"SELECT {WarpField(schema.PrimaryKey)} FROM {WarpField(name)} WHERE {whereCaluse}";
            using (var connection = CreateConnection())
            {
                return connection.ExecuteScalar(sql, data);
            }
        }

        public virtual object Append(string name, object data, RelationalSchema schema, bool returnId = false)
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

            var columns = string.Join(",", dic.Select(s => $@"{WarpField(s.Key)}"));
            var values = string.Join(",", dic.Select(s => $"@{s.Key}"));
            var sql = $@"INSERT INTO {WarpField(name)} ({columns}) VALUES ({values})";

            if (!returnId)
            {
                using (var connection = CreateConnection())
                {
                    connection.Execute(sql, data);
                    return null;
                }
            }

            var whereCaluse = String.Join(" and ", dic.Select(o => $"{WarpField(o.Key)}=@{o.Key}"));
            sql += StatementDelimiter + $"SELECT {WarpField(schema.PrimaryKey)} FROM {WarpField(name)} WHERE {whereCaluse}";
            using (var connection = CreateConnection())
            {
                return connection.ExecuteScalar(sql, data);
            }
        }

        public virtual void CreateIndex(string name, string fieldname)
        {
            var sql = $@"CREATE INDEX {name}_{fieldname} on {WarpField(name)}({WarpField(fieldname)})";

            using (var connection = CreateConnection())
            {
                connection.Execute(sql);
            }
        }

        public virtual void Delete(string name, string primaryKey, object id)
        {
            var sql = $@"DELETE FROM {WarpField(name)} WHERE {primaryKey} = @Id";

            using (var connection = CreateConnection())
            {
                connection.Execute(sql, new { Id = id });
            }
        }

        public virtual void UpdateData(string name, string primaryKey, object id, object data)
        {
            var dic = data as IDictionary<string, object>;
            var keyValues = string.Join(",", dic.Select(s => $@"{WarpField(s.Key)}=@{s.Key}"));
            dic.Add(primaryKey, id);
            var sql = $@"UPDATE {WarpField(name)} SET {keyValues} WHERE {primaryKey} = @{primaryKey}";

            using (var connection = CreateConnection())
            {
                connection.Execute(sql, data);
            }
        }

        public abstract RelationModel GetRelation(string name, string relation);

        public virtual object[] QueryData(string name, string where = null, long? limit = null, long? offset = null, string orderBy = null, object @params = null)
        {
            var conditions = QueryPraser.ParseConditoin(where);
            var whereStr = where == null ? string.Empty : $"WHERE {ConditionsToSql(conditions)}";
            var limitStr = limit.HasValue ? $"LIMIT {limit}" : string.Empty;
            var orderByStr = orderBy == null ? string.Empty : $"ORDER BY {orderBy}";
            var offsetStr = offset.HasValue && offset != 0 ? $"OFFSET {offset}" : string.Empty;
            var sql = $@"SELECT * FROM {WarpField(name)} {whereStr} {orderByStr} {limitStr} {offsetStr}";

            using (var connection = CreateConnection())
            {
                return connection.Query<object>(sql, @params).ToArray();
            }
        }

        public virtual int Count(string name, string where = null, long? limit = null, long? offset = null)
        {
            var conditions = QueryPraser.ParseConditoin(where);
            var whereStr = where == null ? string.Empty : $"WHERE {ConditionsToSql(conditions)}";
            var sql = $@"SELECT count(*) FROM {WarpField(name)} {whereStr}";
            int count;

            using (var connection = CreateConnection())
            {
                count = connection.Query<int>(sql).FirstOrDefault();
            }

            if (limit.HasValue && count > limit) count = (int)limit.Value;

            if (offset.HasValue)
            {
                count -= (int)offset.Value;
                if (count < 0) count = 0;
            };

            return count;
        }

        internal string ConditionsToSql(List<ConditionItem> conditions)
        {
            return string.Join(" and ", conditions.Select(s => $@" {WarpField(s.Field)} {ComparerToString(s.Comparer)} {ConventValue(s)} "));
        }

        internal string WarpField(string field)
        {
            return $"{QuotationLeft}{field}{QuotationRight}";
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

        static string ConventValue(ConditionItem condition)
        {
            switch (condition.Comparer)
            {
                case Comparer.EqualTo:
                case Comparer.NotEqualTo:
                    if (condition.IsQuoted || (!decimal.TryParse(condition.Value, out _) && !bool.TryParse(condition.Value, out _)))
                    {
                        return $"'{condition.Value}'";
                    }
                    break;
                case Comparer.StartWith:
                    return $"'{condition.Value}%'";
                case Comparer.Contains:
                    return $"'%{condition.Value}%'";
                default:
                    break;
            }

            return condition.Value;
        }
    }
}
