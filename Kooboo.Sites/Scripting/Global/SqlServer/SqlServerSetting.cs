using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class SqlServerSetting : ISiteSetting, ISettingDescription
    {
        public SqlServerSetting()
        {
        }

        public string ConnectionString { get; set; }

        public string Name => "SqlServer";

        public string Group => "Database";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
