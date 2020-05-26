//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
   public class CoreSetting : CoreObject
    {
        public CoreSetting()
        {
            this.ConstType = ConstObjectType.CoreSetting; 
        }
         
        private Dictionary<string, string> _values; 

        public Dictionary<string, string> Values {
            get
            {
                if (_values == null)
                {
                    _values = new Dictionary<string, string>(); 
                }
                return _values; 
            }
            set
            {
                _values = value; 
            }
        }

        public override int GetHashCode()
        {
            string unique = ""; 
            if (_values !=null)
            {
                foreach (var item in _values)
                {
                    unique += item.Key + item.Value; 
                }
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }
}
