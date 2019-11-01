//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Web.ViewModel
{
    public class LabelItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        private Dictionary<string, object> _values;

        public Dictionary<string, object> Values
        {
            get { return _values ?? (_values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)); }
            set
            {
                _values = value;
            }
        }

        public DateTime LastModified
        {
            get; set;
        }

        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }

        public Dictionary<string, int> Relations { get; set; }
    }
}