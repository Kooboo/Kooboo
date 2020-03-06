using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class MysqlDatabase : RelationalDatabase<MysqlExecuter, MysqlSchema, MySqlConnection>
    {
        public MysqlDatabase(string connectionString) : base(connectionString)
        {
        }

        public override string Source => "mysql";
    }
}
