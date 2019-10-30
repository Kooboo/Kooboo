//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.Models
{
    public class CoreSetting : CoreObject
    {
        public CoreSetting()
        {
            this.ConstType = ConstObjectType.CoreSetting;
        }

        private Dictionary<string, string> _values;

        public Dictionary<string, string> Values
        {
            get { return _values ?? (_values = new Dictionary<string, string>()); }
            set
            {
                _values = value;
            }
        }

        public override int GetHashCode()
        {
            string unique = "";
            if (_values != null)
            {
                foreach (var item in _values)
                {
                    unique += item.Key + item.Value;
                }
            }
            return Lib.Security.Hash.ComputeInt(unique);
        }
    }
}