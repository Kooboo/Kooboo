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
SELECT ""table"",""from"",""to"" FROM pragma_foreign_key_list('{name}') where ""table""='{relation}'
UNION ALL
SELECT ""table"",""to"" AS ""from"",""from"" AS ""to"" FROM pragma_foreign_key_list('{relation}') where ""table""='{name}'
";

            using (var connection = CreateConnection())
            {
                var relations = connection.Query<RelationModel>(sql);
                return relations.FirstOrDefault();
            }
        }

        public override RelationalSchema GetSchema(string name)
        {
            var sql = $@"SELECT ""name"",""type"" FROM pragma_table_info ('{name}');";

            using (var connection = CreateConnection())
            {
                var items = connection.Query<RelationalSchema.Item>(sql);
                return new SqliteSchema(items);
            }
        }
    }
}
