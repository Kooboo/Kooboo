//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Context;

namespace Kooboo.Data
{
    public interface IJob
    {
        string Name { get; }

        /// <summary>
        /// The system job are jobs that configured by system coding and should NOT be changed by UI.  
        /// </summary>
        bool IsSystemJob { get; }

        Dictionary<string, Type> Configuration { get; }

        List<JobConfig> Config { get;   }

        [Newtonsoft.Json.JsonIgnore]
        RenderContext Context { get; set; }
        
        void Execute(Guid WebSiteId, Dictionary<string, object> Config); 
    }

    public class JobConfig
    {
        public string Name { get; set; }
        public string ControlType { get; set; }

        public List<string> Values { get; set; }
    }
}
