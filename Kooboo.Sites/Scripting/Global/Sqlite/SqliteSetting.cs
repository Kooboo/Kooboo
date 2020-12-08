using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mysql
{
    public class SqliteSetting : ISiteSetting, ISettingDescription
    {
        public SqliteSetting()
        {
        }

        public bool ForeignKeyConstraint { get; set; }

        public string Name => "Sqlite";

        public string Group => "Database";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
