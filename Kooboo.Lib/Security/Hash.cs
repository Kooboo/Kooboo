//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Security.Cryptography;

namespace Kooboo.Lib.Security
{
    public static class Hash
    {
        public static Guid ComputeGuidIgnoreCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                input = string.Empty;
            }
            input = input.ToLower();

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);

            // create the md5 hash
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(bytes);
            // convert the hash to a Guid
            return new Guid(data);
        }


        public static Guid ComputeHashGuid(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                input = string.Empty;
            }
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);

            // create the md5 hash
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(bytes);
            // convert the hash to a Guid
            return new Guid(data);
        }


        public static Guid ComputeGuid(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return default(Guid);
            }
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(bytes);
            // convert the hash to a Guid
            return new Guid(data);
        }

        public static int ComputeInt(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = " ";
            }
            s = s.ToLower();
            return ComputeIntCaseSensitive(s);

        }

        public static int ComputeIntCaseSensitive(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = " ";
            }
            var chars = s.ToCharArray();
            var lastCharInd = chars.Length - 1;
            var num1 = 0x15051505;
            var num2 = num1;
            var ind = 0;
            while (ind <= lastCharInd)
            {
                var ch = chars[ind];
                var nextCh = ++ind > lastCharInd ? '\0' : chars[ind];
                num1 = (((num1 << 5) + num1) + (num1 >> 0x1b)) ^ (nextCh << 16 | ch);
                if (++ind > lastCharInd) break;
                ch = chars[ind];
                nextCh = ++ind > lastCharInd ? '\0' : chars[ind++];
                num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ (nextCh << 16 | ch);
            }
            return num1 + num2 * 0x5d588b65;
        }

    }
}
