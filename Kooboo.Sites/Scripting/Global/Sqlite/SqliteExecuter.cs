using Dapper;
using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using KScript;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Sqlite
{
    public class SqliteExecuter : SqlExecuter<SQLiteConnection>
    {
        public SqliteExecuter(string connectionSring) : base(connectionSring)
        {
        }

        public override char QuotationLeft => '"';

        public override char QuotationRight => '"';

        public override void CreateTable(string name)
        {
            var sql = $@"CREATE TABLE ""{name}"" ( _id TEXT PRIMARY KEY)";

            using (var connection = CreateConnection())
            {
                connection.Execute(sql);
            }

        }

        public override RelationModel GetRelation(string name, string relation)
        {
            var sql = $@"
SELECT ""table"" AS TableA,""from"",""to"" FROM pragma_foreign_key_list('{name}') where ""table""='{relation}'
UNION ALL
SELECT ""table"" AS TableA,""to"" AS ""from"",""from"" AS ""to"" FROM pragma_foreign_key_list('{relation}') where ""table""='{name}'
";

            using (var connection = CreateConnection())
            {
                var result = connection.Query<RelationModel>(sql).FirstOrDefault();

                if (result != null && result.TableA == relation)
                {
                    result.TableA = name;
                }

                return result;
            }
        }

        public override RelationalSchema GetSchema(string name)
        {
            var sql = $@"SELECT ""name"",""type"", pk AS ""IsPrimaryKey"" FROM pragma_table_info ('{name}');";

            using (var connection = CreateConnection())
            {
                var items = connection.Query<RelationalSchema.Item>(sql);
                return new SqliteSchema(items);
            }
        }
    }
}
