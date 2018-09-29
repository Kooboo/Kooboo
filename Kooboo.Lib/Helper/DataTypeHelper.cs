//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Helper
{
  public    static  class DataTypeHelper
    {
        public static bool IsGuid(string input)
        {
            Guid outid;
            return System.Guid.TryParse(input, out outid); 
        }

        public static bool IsInt(string input)
        {
            long outid;
            return long.TryParse(input, out outid);  
        }

        public static bool IsJsonType(string json, Type type)
        {
            try
            {
                var x = Lib.Helper.JsonHelper.Deserialize(json, type);
                return true; 
            }
            catch (Exception)
            { 
            }

            return false; 
        }

        public static bool IsBool(string input)
        {
            bool OK; 
            return bool.TryParse(input, out OK);  
        }

    }

}