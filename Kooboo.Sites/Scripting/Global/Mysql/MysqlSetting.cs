using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class MysqlSetting : ISiteSetting, ISettingDescription
    {
        public MysqlSetting()
        {
        }

        public string ConnectionString { get; set; }

        public string Name => "Mysql";

        public string Group => "Database";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
