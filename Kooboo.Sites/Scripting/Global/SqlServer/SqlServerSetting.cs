using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class SqlServerSetting : ISiteSetting
    {
        public SqlServerSetting()
        {
        }

        public string ConnectionString { get; set; }

        public string Name => "SqlServer";
    }
}
