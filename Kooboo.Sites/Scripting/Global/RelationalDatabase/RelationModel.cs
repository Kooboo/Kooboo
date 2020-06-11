using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public class RelationModel
    {
        public string TableA { get; set; }
        public string TableB { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
