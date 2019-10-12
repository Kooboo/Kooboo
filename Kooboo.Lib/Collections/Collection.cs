//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Lib
{
    public static class Collection
    {
        public static HashSet<T> CopySet<T>(HashSet<T> input)
        {
            HashSet<T> newSet = new HashSet<T>();
            foreach (var item in input)
            {
                newSet.Add(item);
            }
            return newSet;
        }
    }
}