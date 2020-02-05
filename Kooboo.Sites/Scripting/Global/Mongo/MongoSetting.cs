using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mongo
{
    public class MongoSetting : ISiteSetting
    {
        public string Name => "Mongo";
        public string ConnectionString { get; set; }
    }
}
