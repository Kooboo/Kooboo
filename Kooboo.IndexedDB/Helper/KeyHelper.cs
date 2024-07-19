//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Security.Cryptography;

namespace Kooboo.IndexedDB.Helper
{
    public static class KeyHelper
    {
        public static int GetKeyLen(Type keytype, int len = 0)
        {
            if (keytype == typeof(string))
            {
                if (len == 0)
                {
                    return GlobalSettings.defaultKeyLength;
                }
                else
                {
                    return len;
                }
            }
            else if (keytype == typeof(byte[]))
            {
                if (len == 0)
                {
                    return GlobalSettings.defaultKeyLength;
                }
                else

                {
                    return len;
                }
            }
            else if (keytype == typeof(Int32))
            {
                return 4;
            }
            else if (keytype == typeof(Int64))
            {
                return 8;
            }
            else if (keytype == typeof(Int16))
            {
                return 2;
            }
            else if (keytype == typeof(decimal))
            {
                ///decimal is not available, will be converted to double directly on byteconverter.
                return 8;
            }
            else if (keytype == typeof(double))
            {
                return 8;
            }
            else if (keytype == typeof(float))
            {
                return 4;
            }
            else if (keytype == typeof(DateTime))
            {
                return 8;
            }
            else if (keytype == typeof(Guid))
            {
                return 16;
            }
            else if (keytype == typeof(byte))
            {
                return 1;
            }
            else if (keytype == typeof(bool))
            {
                return 1;
            }
            else if (keytype.IsEnum)
            {
                return 4;
            }
            else
            {
                throw new Exception(keytype.ToString() + " data type not supported");
            }
        }

        public static byte[] AppendToKeyLength(byte[] input, bool IsLenVaries, int KeyLen)
        {
            if (!IsLenVaries)
            {
                return input;
            }
            int currentbytecount = input.Length;

            if (currentbytecount > KeyLen)
            {
                byte[] fixedlenbytes = new byte[KeyLen];
                System.Buffer.BlockCopy(input, 0, fixedlenbytes, 0, KeyLen);
                return fixedlenbytes;
            }
            else if (currentbytecount < KeyLen)
            {
                byte[] fixedlenbytes = new byte[KeyLen];
                System.Buffer.BlockCopy(input, 0, fixedlenbytes, 0, currentbytecount);
                return fixedlenbytes;
            }
            else
            {
                return input;
            }
        }


        public static bool IsKeyLenVar(Type keytype)
        {
            if (keytype == typeof(string) || keytype == typeof(byte[]))
            {
                return true;
            }
            return false;
        }

        public static Guid ComputeGuid(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default(Guid);
                //input = string.Empty;
            }
            input = input.ToLower();

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(input);

            // create the md5 hash
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(bytes);
            // convert the hash to a Guid
            return new Guid(data);
        }

        public static Guid ComputeGuid(byte[] bytes)
        {
            if (bytes == null)
            {
                return default(Guid);
            }
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(bytes);
            return new Guid(data);
        }
    }
}
