using Dapper;
using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using KScript;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Sqlite
{
    public class SqliteExecuter : SqlExecuter
    {
        public SqliteExecuter(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        ~SqliteExecuter()
        {
            Connection.Dispose();
        }

        public override char QuotationLeft => '"';

        public override char QuotationRight => '"';

        public override void CreateIndex(string name, string fieldname)
        {
            Connection.Execute($@"CREATE INDEX {fieldname} on ""{name}""(""{fieldname}"")");
        }

        public override void CreateTable(string name)
        {
            Connection.Execute($@"CREATE TABLE ""{name}"" ( _id TEXT PRIMARY KEY)");
        }

        public override RelationModel GetRelation(string name, string relation)
        {
            var sql = $@"SELECT ""table"",""from"",""to"" FROM pragma_foreign_key_list('{name}') where ""table""='{relation}';";
            var relations = Connection.Query<RelationModel>(sql);
            return relations.FirstOrDefault();
        }

        public override RelationalSchema GetSchema(string name)
        {
            var items = Connection.Query<RelationalSchema.Item>($@"SELECT ""name"",""type"" as _type FROM pragma_table_info ('{name}');");
            return new SqliteSchema(items);
        }

        public override void UpgradeSchema(string name, IEnumerable<RelationalSchema.Item> items)
        {
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                sb.AppendLine($@"ALTER TABLE ""{name}"" ADD COLUMN {item.Name} {item.Type.ToString()};");
            }

            Connection.Execute(sb.ToString());
        }
    }
}
