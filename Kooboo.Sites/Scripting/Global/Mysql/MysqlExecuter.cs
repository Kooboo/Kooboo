using Dapper;
using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class MysqlExecuter : SqlExecuter
    {
        public MysqlExecuter(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public override char QuotationLeft => '`';
        public override char QuotationRight => '`';

        public override void CreateIndex(string name, string fieldname)
        {
            Connection.Execute($@"CREATE INDEX {fieldname} on `{name}`(`{fieldname}`)");
        }

        public override void CreateTable(string name)
        {
            Connection.Execute($@"CREATE TABLE `{name}` ( _id char(36) ,PRIMARY KEY ( `_id` ))ENGINE=InnoDB DEFAULT CHARSET=utf8;");
        }

        public override RelationModel GetRelation(string name, string relation)
        {
            throw new NotImplementedException();
        }

        public override RelationalSchema GetSchema(string name)
        {
           var result= Connection.Query<object>($"DESCRIBE `{name}`");

            return null;
           //var item= result.Select(s => new RelationalSchema.Item { Name = s.field, Type = s.type });
           // return new MysqlSchema(item);
        }

        public override void UpgradeSchema(string name, IEnumerable<RelationalSchema.Item> items)
        {
            throw new NotImplementedException();
        }
    }
}
