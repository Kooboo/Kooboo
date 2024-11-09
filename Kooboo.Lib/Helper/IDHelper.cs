//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;

namespace Kooboo.Lib.Helper
{
    public static class IDHelper
    {
        private static Random RND = new Random();

        private static object _locker = new object();

        private static long _currentId = 100;

        public static long NewLongId()
        {
            lock (_locker)
            {
                _currentId += 1;
                return _currentId;
            }
        }

        public static int NextRandom(int begin, int end)
        {
            return RND.Next(begin, end);
        }


        public static Guid NewTimeGuid(DateTime utcTime)
        {
            var tick = utcTime.Ticks;

            byte[] tickbytes = BitConverter.GetBytes(tick);

            tickbytes = tickbytes.Reverse().ToArray();

            var newid = Guid.NewGuid();

            var idbytes = newid.ToByteArray();

            byte[] lastid = new byte[16];

            System.Buffer.BlockCopy(tickbytes, 0, lastid, 0, 8);

            System.Buffer.BlockCopy(idbytes, 8, lastid, 8, 8);

            return new Guid(lastid);
        }

        public static DateTime ExtractTimeFromGuid(Guid id)
        {
            var bytes = id.ToByteArray();

            byte[] longbyte = new byte[8];

            System.Buffer.BlockCopy(bytes, 0, longbyte, 0, 8);

            longbyte = longbyte.Reverse().ToArray();

            var longtick = BitConverter.ToInt64(longbyte, 0);

            var time = new DateTime(longtick, DateTimeKind.Utc);

            return time;

        }

        // two way int to guid. 
        public static Guid NewIntGuid(int id)
        {
            byte[] intbytes = BitConverter.GetBytes(id);

            intbytes = intbytes.Reverse().ToArray();

            var newid = Security.Hash.ComputeGuidIgnoreCase(id.ToString());

            var idbytes = newid.ToByteArray();

            byte[] lastid = new byte[16];

            System.Buffer.BlockCopy(intbytes, 0, lastid, 0, 4);

            System.Buffer.BlockCopy(idbytes, 0, lastid, 4, 12);

            return new Guid(lastid);
        }

        // two way int to guid. 
        public static Guid NewIntRandomGuid(int id)
        {
            byte[] intbytes = BitConverter.GetBytes(id);

            intbytes = intbytes.Reverse().ToArray();

            var newid = System.Guid.NewGuid();

            var idbytes = newid.ToByteArray();

            byte[] lastid = new byte[16];

            System.Buffer.BlockCopy(intbytes, 0, lastid, 0, 4);

            System.Buffer.BlockCopy(idbytes, 0, lastid, 4, 12);

            return new Guid(lastid);
        }

        public static Int32 ExtractIntFromGuid(Guid id)
        {
            var bytes = id.ToByteArray();

            byte[] intbytes = new byte[4];

            System.Buffer.BlockCopy(bytes, 0, intbytes, 0, 4);

            intbytes = intbytes.Reverse().ToArray();

            return BitConverter.ToInt32(intbytes, 0); 
        }
         
        public static Guid ParseKey(object key)
        {
            if (key == null)
            {
                return default(Guid);
            }

            if (key is System.Guid)
            {
                return (Guid)key;
            }
            string strKey = key.ToString();
            return GetOrParseKey(strKey);
        }

        public static Guid GetOrParseKey(string strKey)
        {
            Guid guidKey;
            if (System.Guid.TryParse(strKey, out guidKey))
            {
                return guidKey;
            }
            else
            {
                return Lib.Security.Hash.ComputeGuidIgnoreCase(strKey);
            }
        }

        public static Guid CombineId(Guid one, Guid two)
        { 
            // Convert the GUIDs to byte arrays
            byte[] bytes1 = one.ToByteArray();
            byte[] bytes2 = two.ToByteArray();

            // Create a new byte array to hold the combined bytes
            byte[] combinedBytes = new byte[16];

            // Combine the two GUIDs by taking the first 8 bytes from each
            Array.Copy(bytes1, 0, combinedBytes, 0, 8); // First half from guid1
            Array.Copy(bytes2, 0, combinedBytes, 8, 8); // Second half from guid2

            return new Guid(combinedBytes);
        }

    }
}
