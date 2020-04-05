using Dapper;
using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class SqlServerExecuter : SqlExecuter<SqlConnection>
    {
        public SqlServerExecuter(string connectionSring) : base(connectionSring)
        {
        }

        public override char QuotationLeft => '[';
        public override char QuotationRight => ']';

        public override void CreateTable(string name)
        {
            var sql = $@"CREATE TABLE [{name}] ( _id uniqueidentifier PRIMARY KEY)";

            using (var connection = CreateConnection())
            {
                connection.Execute(sql);
            }
        }

        public override RelationModel GetRelation(string name, string relation)
        {
            var sql = $@"
SELECT
	fname.name AS [tableA],
	fcol.name AS [from],
	rname.name AS [tableB],
	rcol.name AS [to] 
FROM
	sysforeignkeys fk
	LEFT JOIN sysobjects fkname ON fk.constid= fkname.id
	LEFT JOIN sysobjects fname ON fname.id= fk.fkeyid
	LEFT JOIN sys.syscolumns fcol ON fcol.id = fk.fkeyid 
	AND fcol.colid = fk.fkey
	LEFT JOIN sysobjects rname ON rname.id= fk.rkeyid
	LEFT JOIN sys.syscolumns rcol ON rcol.id = fk.rkeyid 
	AND rcol.colid = fk.rkey 
WHERE
	fkname.name= @Name
";
            using (var connection = CreateConnection())
            {
                var result = connection.Query<RelationModel>(sql, new { Name = name }).FirstOrDefault();

                if (result != null && result.TableA == relation)
                {
                    var to = result.To;
                    result.To = result.From;
                    result.From = to;
                    result.TableA = result.TableB;
                }

                return result;
            }
        }

        public override RelationalSchema GetSchema(string name)
        {
            using (var connection = CreateConnection())
            {
                var result = connection.Query<RelationalSchema.Item>($@"
SELECT
	TCol.COLUMN_NAME AS [name],
	TCol.DATA_TYPE AS [type],
	( CASE TKey.COLUMN_NAME WHEN TCol.COLUMN_NAME THEN 1 ELSE 0 END ) AS IsPrimaryKey 
FROM
	INFORMATION_SCHEMA.columns AS TCol
	LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS TKey 
         ON TKey.TABLE_NAME = TCol.TABLE_NAME AND TKey.COLUMN_NAME = TCol.COLUMN_NAME
WHERE
	TCol.TABLE_NAME= @Name
", new { Name = name });

                return new SqlServerSchema(result);
            }
        }

        public override object[] QueryData(string name, string where = null, long? limit = null, long? offset = null, string orderBy = null, object @params = null)
        {
            var conditions = IndexedDB.Dynamic.QueryPraser.ParseConditoin(where);
            var whereStr = where == null ? string.Empty : $"WHERE {ConditionsToSql(conditions)}";
            var limitStr = limit.HasValue ? $"ROW FETCH NEXT {limit} ROWS ONLY" : string.Empty;
            var orderByStr = orderBy == null ? string.Empty : $"ORDER BY {orderBy}";
            var offsetStr = offset.HasValue && offset != 0 ? $"OFFSET {offset}" : string.Empty;
            if (limit.HasValue) offsetStr = "OFFSET 0";
            var sql = $@"SELECT * FROM {WarpField(name)} {whereStr} {orderByStr} {offsetStr} {limitStr}";

            using (var connection = CreateConnection())
            {
                return connection.Query<object>(sql, @params).ToArray();
            }
        }

        public override void UpgradeSchema(string name, IEnumerable<RelationalSchema.Item> items)
        {
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                var length = item.Length > 0 ? $"({item.Length})" : string.Empty;
                sb.AppendLine($@"ALTER TABLE {WarpField(name)} ADD {WarpField(item.Name)} {item.Type.ToString()}{length};");
            }

            using (var connection = CreateConnection())
            {
                connection.Execute(sb.ToString());
            }
        }
    }
}
