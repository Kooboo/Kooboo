//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Models
{
    public class BackendLog
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        private Dictionary<string, string> _data;

        public Dictionary<string, string> Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new Dictionary<string, string>();
                }
                return _data;
            }
            set => _data = value;
        }

        public string Url { get; set; }

        public string IP { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.Now;

        public int StatusCode { get; set; } = 200;

        public bool IsApi { get; set; }
    }
}