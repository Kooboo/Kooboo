//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Config
{
    /// <summary>
    /// generate sequence id for incremental integer. 
    /// </summary>
    public static class SequenceId
    {

        private static object _object = new object();

        private static object _setobject = new object();

        private static Dictionary<string, Int64> _long;
        private static Dictionary<string, Int64> LongList
        {
            get
            {
                if (_long == null)
                {
                    lock (_object)
                    {
                        _long = new Dictionary<string, long>();
                    }
                }
                return _long;
            }
        }

        public static void SetLong(string name, Int64 value)
        {
            lock (_setobject)
            {
                //if (LongList.ContainsKey(name))
                //{
                LongList[name] = value;
                //}
                //else
                //{
                //    LongList.Add(name, value); 
                //}
            }
        }

        private static Int64 GetLong(string name)
        {
            if (LongList.ContainsKey(name))
            {
                return LongList[name];
            }
            else
            {
                return 0;
            }
        }

        public static Int64 GetNewLongId(string name)
        {
            lock (_object)
            {
                Int64 currentvalue = GetLong(name);
                currentvalue += 1;
                SetLong(name, currentvalue);
                return currentvalue;
            }
        }

    }
}
