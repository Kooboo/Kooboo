using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using KScript;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Sqlite
{
    public class SqliteDatabase : RelationalDatabase<SqliteExecuter, SqliteSchema, SqliteConnection>
    {
        public SqliteDatabase(string connectionString) : base(connectionString)
        {
        }
    }
}
