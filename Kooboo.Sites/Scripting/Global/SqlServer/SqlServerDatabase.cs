using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class SqlServerDatabase : RelationalDatabase<SqlServerExecuter, SqlServerSchema, MySqlConnection>
    {
        public SqlServerDatabase(string connectionString) : base(connectionString)
        {
        }
    }
}
