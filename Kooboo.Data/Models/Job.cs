//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Models
{
    [Serializable]
    public class Job
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid();
                }
                return _id;
            }
            set
            {

                _id = value;
            }
        }

        public string Description { get; set; }
         
        public Guid WebSiteId { get; set; }

        private Dictionary<string, object> _config; 

        [Obsolete]
        public Dictionary<string, object> Config {
            get {
                if (_config == null)
                {
                    _config = new Dictionary<string, object>(); 
                }
                return _config; 
            }
            set {
                _config = value; 
            }
        }

        public string JobName { get; set; }

        // the K Script
        public string Script { get; set; }

        public Guid CodeId { get; set; }
    }
}
