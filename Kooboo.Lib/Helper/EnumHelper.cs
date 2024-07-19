//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

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

            if (rightname != null)
            {
                if (Enum.TryParse<T>(rightname, out T result))
                {
                    return result;
                }
            }
            return default(T);
        }

        public static object GetEnum(Type type, string name)
        {
            var rightname = GetRightName(type, name);

            if (rightname != null)
            {
                try
                {
                    return Enum.Parse(type, rightname);
                }
                catch (Exception)
                {

                }
            }
            return null;
        }

        private static string GetRightName(Type type, string value)
        {
            var names = Enum.GetNames(type);

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

        public static Dictionary<string, int> GetEnumDict(Type en)
        {
            Dictionary<string, int> enumDic = new Dictionary<string, int>();

            string[] enumKey = Enum.GetNames(en);
            int[] enumValue = new int[enumKey.Length];
            Enum.GetValues(en).CopyTo(enumValue, 0);
            for (int i = 0; i < enumKey.Length; i++)
            {
                enumDic.Add(enumKey[i], enumValue[i]);
            }
            return enumDic;
        }

    }
}
