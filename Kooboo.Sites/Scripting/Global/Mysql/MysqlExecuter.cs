using Dapper;
using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class MysqlExecuter : SqlExecuter<MySqlConnection>
    {
        public MysqlExecuter(string connectionSring) : base(connectionSring)
        {
        }

        public override char QuotationLeft => '`';
        public override char QuotationRight => '`';

        public override void CreateTable(string name)
        {
            var sql = $@"CREATE TABLE `{name}` ( _id char(36) ,PRIMARY KEY ( `_id` ))ENGINE=InnoDB DEFAULT CHARSET=utf8;";

            using (var connection = CreateConnection())
            {
                connection.Execute(sql);
            }
        }

        public override RelationModel GetRelation(string name, string relation)
        {
            var sql = $"select TABLE_NAME as `tableA`,REFERENCED_TABLE_NAME as `tableB`,COLUMN_NAME as `from`,REFERENCED_COLUMN_NAME as `to` from INFORMATION_SCHEMA.KEY_COLUMN_USAGE  where CONSTRAINT_NAME='{name}'";
            using (var connection = CreateConnection())
            {
                var result = connection.Query<RelationModel>(sql).FirstOrDefault();

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
            IEnumerable<RelationalSchema.Item> items = null;

            try
            {
                using (var connection = CreateConnection())
                {
                    var result = connection.Query<object>($"DESCRIBE `{name}`");

                    items = result.Select(s =>
                    {
                        var dic = s as IDictionary<string, object>;
                        return new RelationalSchema.Item
                        {
                            Name = dic["Field"].ToString(),
                            Type = dic["Type"].ToString(),
                            IsPrimaryKey = dic["Key"].ToString() == "PRI"
                        };
                    });
                }
            }
            catch (Exception)
            {
                items = new RelationalSchema.Item[0];
            }

            return new MysqlSchema(items);
        }
    }
}
