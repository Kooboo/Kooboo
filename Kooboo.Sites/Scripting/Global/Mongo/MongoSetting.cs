using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KScript
{
    public class MongoSetting : ISiteSetting, ISettingDescription
    {
        public string Name => "Mongo";
        public string ConnectionString { get; set; }

        public string Group => "Database";

        public string GetAlert(RenderContext renderContext)
        {
            return $@"mongodb://[username:password@]host1[:port1][,host2[:port2],…[,hostN[:portN]]][/[database][?options]]

Example:
mongodb://admin:123@localhost/mydb
";
        }
    }
}
