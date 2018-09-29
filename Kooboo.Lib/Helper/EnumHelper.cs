//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Helper
{
   public static class EnumHelper
    {
        // use when the value can be lower or upper case. 
        public static string GetRightName<TEnum>(string value)
        {
            var names = Enum.GetNames(typeof(TEnum));

            string lower = value.ToLower();

            if (names == null)
            {
                return null;
            }

            foreach (var item in names)
            {
                if (item.ToLower() == lower)
                {
                    return item;
                }
            }
            return null;
        }

        public static T GetEnum<T>(string name) where T : struct, IConvertible
        {
            var rightname = GetRightName<T>(name); 
           
            if (rightname !=null)
            {
                if (Enum.TryParse<T>(rightname, out T result))
                {
                    return result; 
                }
            }
            return default(T); 
        }     
    }
}
