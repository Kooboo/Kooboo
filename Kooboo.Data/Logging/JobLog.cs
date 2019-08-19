//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
 

namespace Kooboo.Data
{
    /// <summary>
    /// The log of schedule or repeating job execution result. 
    /// </summary>
    public class JobLog
    {
        public string JobName { get; set; }

        public DateTime ExecutionTime { get; set; }

        public bool Success { get; set; }

        public string Description { get; set; }

        public string Message { get; set; }
        public Guid WebSiteId { get; set; }
    }
}
