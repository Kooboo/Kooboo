//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Lib.Reflection
{
    public static class ParameterHelper
    {

        public static object DefaultValue(Type clrtype)
        {
            if (clrtype == typeof(string))
            {
                return "";
            }
            else if (clrtype == typeof(byte) || clrtype == typeof(int) || clrtype == typeof(Int16) || clrtype == typeof(long) || clrtype == typeof(decimal) || clrtype == typeof(double) || clrtype == typeof(float))
            {
                return Convert.ChangeType(0, clrtype);
            }
            else if (clrtype == typeof(Guid))
            {
                return default(Guid);
            }

            else if (clrtype == typeof(bool))
            {
                return false;
            }
            else if (clrtype == typeof(DateTime))
            {
                return DateTime.Now;
            }
            return null;
        }


    }
}
