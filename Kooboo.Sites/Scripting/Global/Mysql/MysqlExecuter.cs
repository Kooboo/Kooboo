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
            IEnumerable<RelationalSchema.Item> items = null;

            try
            {
                var result = Connection.Query<object>($"DESCRIBE `{name}`");

                items = result.Select(s =>
                   {
                       var dic = s as IDictionary<string, object>;
                       return new RelationalSchema.Item { Name = dic["Field"].ToString(), Type = dic["Type"].ToString() };
                   });
            }
            catch (Exception)
            {
                items = new RelationalSchema.Item[0];
            }

            return new MysqlSchema(items);
        }
    }
}
