//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Cache
{
    public static class RenderCache
    {
        private static object _bianrylocker = new object();

        private static object _htmlcachelock = new object();

        private static Dictionary<Guid, string> _htmlCache = new Dictionary<Guid, string>();

        private static Dictionary<Guid, byte[]> _binaryCache = new Dictionary<Guid, byte[]>();

        public static string GetHtml(Guid key)
        {
            return _htmlCache.ContainsKey(key) ? _htmlCache[key] : null;
        }

        public static void SetHtml(Guid key, string source)
        {
            if (!_htmlCache.ContainsKey(key))
            {
                lock (_htmlcachelock)
                {
                    if (!_htmlCache.ContainsKey(key))
                    {
                        _htmlCache[key] = source;
                    }
                }
            }
        }

        public static byte[] GetBinary(Guid key)
        {
            if (_binaryCache.ContainsKey(key))
            {
                return _binaryCache[key];
            }
            return null;
        }

        public static void SetBinary(Guid key, byte[] source)
        {
            if (!_binaryCache.ContainsKey(key))
            {
                lock (_htmlcachelock)
                {
                    if (!_binaryCache.ContainsKey(key))
                    {
                        _binaryCache[key] = source;
                    }
                }
            }
        }
    }
}