using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class SqlServerDatabase : RelationalDatabase<SqlServerExecuter, SqlServerSchema, SqlConnection>
    {
        public SqlServerDatabase(string connectionString) : base(connectionString)
        {
        }

        public override string Source => "sqlserver";
    }
}
