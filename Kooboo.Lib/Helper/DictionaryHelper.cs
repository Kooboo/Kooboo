//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Lib.Helper
{
    public static class DictionaryHelper
    {
        public static string GetString(Dictionary<string, string> dict, string key)
        {
            if (dict == null)
            {
                return null;
            }
            string lower = key.ToLower();
            foreach (var item in dict)
            {
                if (item.Key.ToLower() == lower)
                {
                    return item.Value;
                }
            }
            return null;
        }
    }
}