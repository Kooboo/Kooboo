using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using KScript;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Sqlite
{
    public class SqliteDatabase : RelationalDatabase<SqliteExecuter, SqliteSchema, SQLiteConnection>
    {
        public SqliteDatabase(string connectionString) : base(connectionString)
        {
        }

        public override string Source => "sqlite";
    }
}
