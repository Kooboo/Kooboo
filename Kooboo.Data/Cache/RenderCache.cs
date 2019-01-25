//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Cache
{
   public static class RenderCache
    {
        private static object _bianrylocker = new object();

        private static object _htmlcachelock = new object();

        private static Dictionary<Guid, string> HtmlCache = new Dictionary<Guid, string>();

        private static Dictionary<Guid, byte[]> BinaryCache = new Dictionary<Guid, byte[]>(); 
          
        public static string GetHtml(Guid Key)
        {
            if (HtmlCache.ContainsKey(Key))
            {
                return HtmlCache[Key]; 
            } 
            return null;
        }
        public static void SetHtml(Guid Key, string source)
        {
            if (!HtmlCache.ContainsKey(Key))
            {
                lock (_htmlcachelock)
                {
                    if (!HtmlCache.ContainsKey(Key))
                    {
                        HtmlCache[Key] = source; 
                    }
                }
            }
        }
         
        public static byte[] GetBinary(Guid Key)
        {
            if (BinaryCache.ContainsKey(Key))
            {
                return BinaryCache[Key];
            }
            return null;
        }
        public static void SetBinary(Guid Key, byte[] source)
        {
            if (!BinaryCache.ContainsKey(Key))
            {
                lock (_htmlcachelock)
                {
                    if (!BinaryCache.ContainsKey(Key))
                    {
                        BinaryCache[Key] = source;
                    }
                }
            }
        }  
    }
      
}
