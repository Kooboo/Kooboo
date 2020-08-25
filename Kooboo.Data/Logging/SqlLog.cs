using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Logging
{
    public class SqlLog
    {
        public string Type { get; set; }
        public string Sql { get; set; }
        public string Params { get; set; }
        public Guid SiteId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
