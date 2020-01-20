using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class MysqlDatabase : RelationalDatabase<MysqlExecuter, MysqlSchema>
    {
        public MysqlDatabase(IDbConnection dbConnection) : base(dbConnection)
        {
        }
    }
}
